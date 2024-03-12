// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TriviaService.Test;

[TestClass]
public class OpenTriviaDatabaseTest
{
    [TestMethod]
    public async Task OpenTriviaDatabase_GetQuestionsAsync_DefaultTest()
    {
        var results = await OpenTriviaDatabase.GetQuestionsAsync();
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.Count);
    }

    [TestMethod]
    public async Task OpenTriviaDatabase_GetQuestionsAsync_AmountTest()
    {
        var args = new OpenTriviaDatabase.GetQuestionsArgs()
        {
            Amount = 20,
        };

        var results = await OpenTriviaDatabase.GetQuestionsAsync(args);
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.Count);

        Assert.AreEqual(args.Amount, results.Count);
    }

    [TestMethod]
    public async Task OpenTriviaDatabase_GetQuestionsAsync_CategoryTest()
    {
        var args = new OpenTriviaDatabase.GetQuestionsArgs()
        {
            Category = QuestionCategory.GeneralKnowledge,
        };

        var results = await OpenTriviaDatabase.GetQuestionsAsync(args);
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.Count);

        foreach (var result in results)
        {
            Assert.AreEqual(args.Category, result.Category);
        }
    }

    [TestMethod]
    public async Task OpenTriviaDatabase_GetQuestionsAsync_DifficultyTest()
    {
        var args = new OpenTriviaDatabase.GetQuestionsArgs()
        {
            Difficulty = QuestionDifficulty.Easy,
        };

        var results = await OpenTriviaDatabase.GetQuestionsAsync(args);
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.Count);

        foreach (var result in results)
        {
            Assert.AreEqual(args.Difficulty, result.Difficulty);
        }
    }

    [TestMethod]
    public async Task OpenTriviaDatabase_GetQuestionsAsync_TypeTest()
    {
        var args = new OpenTriviaDatabase.GetQuestionsArgs()
        {
            Type = QuestionType.MultipleChoice,
        };

        var results = await OpenTriviaDatabase.GetQuestionsAsync(args);
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.Count);

        foreach (var result in results)
        {
            Assert.AreEqual(args.Type, result.Type);
        }
    }
}