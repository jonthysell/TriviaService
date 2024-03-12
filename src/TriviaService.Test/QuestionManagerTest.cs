// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TriviaService.Test;

[TestClass]
public class QuestionManagerTest
{
    [TestMethod]
    public void QuestionManager_NewTest()
    {
        var qm = new QuestionManager();
        Assert.IsNotNull(qm);

        Assert.AreEqual(0, qm.QuestionPool.Count);
        Assert.AreEqual(0, qm.AnsweredQuestions.Count);
    }

    [TestMethod]
    public async Task QuestionManager_AnswerQuestionCorrectlyTest()
    {
        var qm = new QuestionManager();
        Assert.IsNotNull(qm);

        Assert.AreEqual(0, qm.QuestionPool.Count);
        Assert.AreEqual(0, qm.AnsweredQuestions.Count);

        var question = await qm.GetNextQuestionAsync();

        bool eventFired = false;
        qm.QuestionAnswered += (s, e) =>
        {
            Assert.AreEqual(question, e.Question);
            Assert.AreEqual(question.CorrectAnswer, e.Answer);
            Assert.IsTrue(e.IsCorrect);
            eventFired = true;
        };

        qm.AnswerQuestion(question, question.CorrectAnswer);

        Assert.AreEqual(1, qm.AnsweredQuestions.Count);

        Assert.IsTrue(eventFired);
    }

    [TestMethod]
    public async Task QuestionManager_AnswerQuestionIncorrectlyTest()
    {
        var qm = new QuestionManager();
        Assert.IsNotNull(qm);

        Assert.AreEqual(0, qm.QuestionPool.Count);
        Assert.AreEqual(0, qm.AnsweredQuestions.Count);

        var question = await qm.GetNextQuestionAsync();

        bool eventFired = false;
        qm.QuestionAnswered += (s, e) =>
        {
            Assert.AreEqual(question, e.Question);
            Assert.AreEqual(question.IncorrectAnswers[0], e.Answer);
            Assert.IsFalse(e.IsCorrect);
            eventFired = true;
        };

        qm.AnswerQuestion(question, question.IncorrectAnswers[0]);

        Assert.AreEqual(1, qm.AnsweredQuestions.Count);

        Assert.IsTrue(eventFired);
    }

}