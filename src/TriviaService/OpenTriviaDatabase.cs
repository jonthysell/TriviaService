// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace TriviaService;

public static class OpenTriviaDatabase
{
    private const string BaseUri = "https://opentdb.com/api.php";

    public static async Task<IList<TriviaQuestion>> GetQuestionsAsync(OpenTriviaDataBaseQuery query)
    {
        var uriSB = new StringBuilder(BaseUri);

        uriSB.Append($"?amount={ query.Amount }");

        if (query.Category.HasValue)
        {
            uriSB.Append($"&category={ ToQueryValue(query.Category.Value) }");
        }

        if (query.Difficulty.HasValue)
        {
            uriSB.Append($"&difficulty={ ToQueryValue(query.Difficulty.Value) }");
        }

        if (query.Type.HasValue)
        {
            uriSB.Append($"&type={ ToQueryValue(query.Type.Value) }");
        }

        var client = new HttpClient();
        var jsonDoc = await client.GetFromJsonAsync<JsonDocument>(uriSB.ToString());

        if (jsonDoc is null)
        {
            throw new Exception($"Unable to parse JSON from response to \"{ uriSB }\".");
        }

        var results = jsonDoc.RootElement.GetProperty("results").EnumerateArray().Select(ParseJson).ToList();

        if (results is null)
        {
            throw new Exception($"No results in response to \"{ uriSB }\", response_code: { (jsonDoc.RootElement.TryGetProperty("response_code", out var responseCode) ? responseCode.ToString() : "???") }");
        }

        return results;
    }

    public static TriviaQuestion ParseJson(JsonElement element)
    {
        var category = GetCategory(element.GetProperty("category").GetString());
        var difficulty = GetDifficulty(element.GetProperty("difficulty").GetString());
        var type = GetType(element.GetProperty("type").GetString());

        var question = element.GetProperty("question").GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".question is null or undefined");
        var correctAnswer = element.GetProperty("correct_answer").GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".correct_answer is null or undefined");

        var incorrectAnswers = element.GetProperty("incorrect_answers").EnumerateArray().Select(answer => answer.GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".incorrect_answers[i] is null or undefined")).ToList() ?? throw new ArgumentOutOfRangeException(nameof(element), ".incorrect_answers is null or undefined");

        return new TriviaQuestion(category, difficulty, type, question, correctAnswer, incorrectAnswers);
    }

    public static bool TryParseJson(JsonElement element, out TriviaQuestion? result)
    {
        try
        {
            result = ParseJson(element);
            return true;
        }
        catch {}

        result = default;
        return false;
    }

    private static QuestionCategory GetCategory(string? jsonValue)
    {
        if (jsonValue is null)
        {
            throw new ArgumentNullException(nameof(jsonValue));
        }

        switch (jsonValue)
        {
            case "General Knowledge":
                return QuestionCategory.GeneralKnowledge;
        }

        throw new ArgumentOutOfRangeException(nameof(jsonValue));
    }

    private static string ToQueryValue(QuestionCategory value)
    {
        return ((int)value).ToString();
    }

    private static QuestionDifficulty GetDifficulty(string? jsonValue)
    {
        if (jsonValue is null)
        {
            throw new ArgumentNullException(nameof(jsonValue));
        }

        switch (jsonValue)
        {
            case "easy":
                return QuestionDifficulty.Easy;
            case "medium":
                return QuestionDifficulty.Medium;
            case "hard":
                return QuestionDifficulty.Hard;
        }

        throw new ArgumentOutOfRangeException(nameof(jsonValue));
    }

    private static string ToQueryValue(QuestionDifficulty value)
    {
        return value.ToString().ToLower();
    }

    private static QuestionType GetType(string? jsonValue)
    {
        if (jsonValue is null)
        {
            throw new ArgumentNullException(nameof(jsonValue));
        }

        switch (jsonValue)
        {
            case "multiple":
                return QuestionType.MultipleChoice;
            case "boolean":
                return QuestionType.TrueFalse;
        }

        throw new ArgumentOutOfRangeException(nameof(jsonValue));
    }

    private static string ToQueryValue(QuestionType value)
    {
        switch (value)
        {
            case QuestionType.MultipleChoice:
                return "multiple";
            case QuestionType.TrueFalse:
                return "boolean";

        }

        throw new ArgumentOutOfRangeException(nameof(value));
    }
}

public struct OpenTriviaDataBaseQuery
{
    public int Amount {get ; set; } = 10;

    public QuestionCategory? Category {get ; set; } = null;

    public QuestionDifficulty? Difficulty {get ; set; }= null;

    public QuestionType? Type {get ; set; } = null;

    public OpenTriviaDataBaseQuery() { }
}