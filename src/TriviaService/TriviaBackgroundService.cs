// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

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

    public TriviaBackgroundService(ILogger<TriviaBackgroundService> logger)
    {
        _logger = logger;

        QuestionManager = new QuestionManager(TimeSpan.FromMinutes(MinutesBetweenInteractions));
        QuestionManager.QuestionAsked += RaiseToastToAskQuestion;
        QuestionManager.QuestionAnswered += RaiseToastForQuestionAnswered;
    }

    private void RaiseToastToAskQuestion(object sender, QuestionAskedEventArgs e)
    {
        LogInfo("Ask: \"{Question}\"", e.Question);
        QuestionManager.AnswerQuestion(e.Question, e.Question.Answers[0]);

    }

    private void RaiseToastForQuestionAnswered(object sender, QuestionAnsweredEventArgs e)
    {
        LogInfo("Answer: \"{Question}\" with \"{Answer}\" which is {Correctness}", e.Question, e.Answer, e.IsCorrect ? "correct" : "incorrect");
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
            LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }
    }

    private void LogError(Exception ex, string? message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message, args);
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
