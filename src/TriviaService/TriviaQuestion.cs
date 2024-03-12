// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TriviaService;

internal class TriviaQuestion
{
    public readonly QuestionCategory Category;
    public readonly QuestionDifficulty Difficulty;
    public readonly QuestionType Type;

    public readonly string Question;

    public readonly string CorrectAnswer;

    public readonly IReadOnlyList<string> IncorrectAnswers;

    public TriviaQuestion(QuestionCategory category, QuestionDifficulty difficulty, QuestionType type, string question, string correctAnswer, IEnumerable<string> incorrectAnswers)
    {
        Category = category;
        Difficulty = difficulty;
        Type = type;

        Question = question;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = new List<string>(incorrectAnswers);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Question, CorrectAnswer);
    }
}