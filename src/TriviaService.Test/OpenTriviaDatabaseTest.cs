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
    }
}