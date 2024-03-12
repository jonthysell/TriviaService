// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TriviaService;

internal class QuestionManager
{
    public TimeSpan InteractionTimeout { get; private set; } = TimeSpan.FromMinutes(60);

    public DateTime LastInteractionTimestamp { get; private set; } = DateTime.MinValue;

    public TimeSpan TimeSinceLastInteraction => DateTime.Now - LastInteractionTimestamp;

    internal HashSet<TriviaQuestion> AnsweredQuestions { get; private set; } = new HashSet<TriviaQuestion>();

    internal Queue<TriviaQuestion> QuestionPool { get; private set; } = new Queue<TriviaQuestion>();

    public event QuestionAskedEventHandler? QuestionAsked;

    public event QuestionAnsweredEventHandler? QuestionAnswered;

    public QuestionManager(TimeSpan interactionTimeout) : this()
    {
        InteractionTimeout = interactionTimeout;
    }

    public QuestionManager() { }

    public async Task RunAsync()
    {
        if (TimeSinceLastInteraction > InteractionTimeout)
        {
            var question = await GetNextQuestionAsync();
            AskQuestion(question);
        }
    }

    internal async Task<TriviaQuestion> GetNextQuestionAsync()
    {
        while (QuestionPool.Count == 0)
        {
            var newQuestions = await OpenTriviaDatabase.GetQuestionsAsync();

            foreach (var newQuestion in newQuestions)
            {
                if (!AnsweredQuestions.Contains(newQuestion))
                {
                    QuestionPool.Enqueue(newQuestion);
                }
            }
        }

        return QuestionPool.Dequeue();
    }

    internal void AskQuestion(TriviaQuestion question)
    {
        LastInteractionTimestamp = DateTime.Now;
        OnQuestionAsked(question);
    }

    protected void OnQuestionAsked(TriviaQuestion question)
    {
        QuestionAsked?.Invoke(this, new QuestionAskedEventArgs(question));
    }

    public void AnswerQuestion(TriviaQuestion question, string answer)
    {
        LastInteractionTimestamp = DateTime.Now;
        if (AnsweredQuestions.Add(question))
        {
            OnQuestionAnswered(question, answer);
        }
    }

    public void DismissQuestion(TriviaQuestion question)
    {
        LastInteractionTimestamp = DateTime.Now;
        AnsweredQuestions.Add(question);
    }

    protected void OnQuestionAnswered(TriviaQuestion question, string answer)
    {
        QuestionAnswered?.Invoke(this, new QuestionAnsweredEventArgs(question, answer));
    }
}

internal delegate void QuestionAskedEventHandler(object sender, QuestionAskedEventArgs e);

internal class QuestionAskedEventArgs : EventArgs
{
    public readonly TriviaQuestion Question;

    public QuestionAskedEventArgs(TriviaQuestion question)
    {
        Question = question;
    }
}

internal delegate void QuestionAnsweredEventHandler(object sender, QuestionAnsweredEventArgs e);

internal class QuestionAnsweredEventArgs : QuestionAskedEventArgs
{
    public readonly string Answer;

    public bool IsCorrect => Answer == Question.CorrectAnswer;

    public QuestionAnsweredEventArgs(TriviaQuestion question, string answer) : base(question)
    {
        Answer = answer;
    }
}
