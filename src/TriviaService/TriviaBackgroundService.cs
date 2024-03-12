// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace TriviaService;

public class TriviaBackgroundService : BackgroundService
{
#if DEBUG
    internal readonly int MinutesBetweenInteractions = 1;
#else
    internal readonly int MinutesBetweenInteractions = 60;
#endif
    internal QuestionManager QuestionManager { get; private set; }


    private readonly ILogger<TriviaBackgroundService> _logger;

    private Dictionary<int, TriviaQuestion> _pendingQuestions = new Dictionary<int, TriviaQuestion>();

    private bool _registeredForNotifications = false;

    public TriviaBackgroundService(ILogger<TriviaBackgroundService> logger)
    {
        _logger = logger;

        QuestionManager = new QuestionManager(TimeSpan.FromMinutes(MinutesBetweenInteractions));
        QuestionManager.QuestionAsked += RaiseToastToAskQuestion;
        QuestionManager.QuestionAnswered += RaiseToastForQuestionAnswered;

        RegisterForNotifications();
    }

    private void RaiseToastToAskQuestion(object sender, QuestionAskedEventArgs e)
    {
        LogInfo("Ask: \"{Question}\"", e.Question);

        var builder = new AppNotificationBuilder()
            .AddArgument("QuestionId", e.Question.GetHashCode().ToString())
            .AddText($"Trivia: {e.Question.Category} ({e.Question.Difficulty})")
            .AddText($"Q: {e.Question.Question} ")
            .AddText($"A:");

        foreach (var answer in e.Question.Answers)
        {
            builder.AddButton(new AppNotificationButton(answer)
                .AddArgument("QuestionId", e.Question.GetHashCode().ToString())
                .AddArgument("Answer", answer));
        }

        var appNotification = builder.BuildNotification();

        AppNotificationManager.Default.Show(appNotification);

        _pendingQuestions.Add(e.Question.GetHashCode(), e.Question);
    }

    private void RaiseToastForQuestionAnswered(object sender, QuestionAnsweredEventArgs e)
    {
        LogInfo("Answer: \"{Question}\" with \"{Answer}\" which is {Correctness}", e.Question, e.Answer, e.IsCorrect ? "correct" : "incorrect");

        var builder = new AppNotificationBuilder()
            .AddText($"{(e.IsCorrect ? "Correct" : "Incorrect")}!")
            .AddText($"Q: {e.Question} ")
            .AddText($"A: {e.Question.CorrectAnswer} ");

        var appNotification = builder.BuildNotification();

        AppNotificationManager.Default.Show(appNotification);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await QuestionManager.RunAsync();
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            LogError(ex);
            UnregisterForNotifications();
            Environment.Exit(1);
        }
    }

    ~TriviaBackgroundService()
    {
        UnregisterForNotifications();
    }

    private void RegisterForNotifications()
    {
        AppNotificationManager.Default.NotificationInvoked += Default_NotificationInvoked;

        AppNotificationManager.Default.Register();
        _registeredForNotifications = true;
    }

    private void Default_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        if (args.Arguments.TryGetValue("QuestionId", out var questionIdStr) && int.TryParse(questionIdStr, out var questionId))
        {
            if (_pendingQuestions.TryGetValue(questionId, out var question))
            {
                if (args.Arguments.TryGetValue("Answer", out var answer))
                {
                    LogInfo("User answering \"{Question}\"", question);
                    QuestionManager.AnswerQuestion(question, answer);
                }
                else
                {
                    LogInfo("User dismissing \"{Question}\"", question);
                    QuestionManager.DismissQuestion(question);
                }

                _pendingQuestions.Remove(questionId);
            }
        }
    }

    private void UnregisterForNotifications()
    {
        if (_registeredForNotifications)
        {
            AppNotificationManager.Default.Unregister();
            _registeredForNotifications = false;
        }
    }

    private void LogError(Exception ex)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }
    }

    private void LogInfo(string? message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message, args);
        }
    }
}
