// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TriviaService;

internal class QuestionManager
{
    public DateTime LastQuestionTimestamp { get; private set; } = DateTime.MinValue;

    public TimeSpan TimeSinceLastQuestionAnswered => DateTime.Now - LastQuestionTimestamp;

    internal HashSet<TriviaQuestion> AnsweredQuestions { get; private set; } = new HashSet<TriviaQuestion>();

    internal Queue<TriviaQuestion> QuestionPool { get; private set; } = new Queue<TriviaQuestion>();

    public event QuestionAnsweredEventHandler? QuestionAnswered;

    public QuestionManager() { }

    public async Task<TriviaQuestion> GetNextQuestionAsync()
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

    public void AnswerQuestion(TriviaQuestion question, string answer)
    {
        if (AnsweredQuestions.Add(question))
        {
            OnQuestionAnswered(question, answer);
        }
    }

    protected void OnQuestionAnswered(TriviaQuestion question, string answer)
    {
        QuestionAnswered?.Invoke(this, new QuestionAnsweredEventArgs(question, answer));
    }
}

internal delegate void QuestionAnsweredEventHandler(object sender, QuestionAnsweredEventArgs e);

internal class QuestionAnsweredEventArgs : EventArgs
{
    public readonly TriviaQuestion Question;

    public readonly string Answer;

    public bool IsCorrect => Answer == Question.CorrectAnswer;

    public QuestionAnsweredEventArgs(TriviaQuestion question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}
