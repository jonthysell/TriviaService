// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TriviaService;

internal static class OpenTriviaDatabase
{
    private const string BaseUri = "https://opentdb.com/api.php";

    public static async Task<IList<TriviaQuestion>> GetQuestionsAsync() => await GetQuestionsAsync(new GetQuestionsArgs());
    public static async Task<IList<TriviaQuestion>> GetQuestionsAsync(GetQuestionsArgs query)
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

        JsonDocument? jsonDoc = null;
        while (jsonDoc is null)
        {
            try
            {
                jsonDoc = await client.GetFromJsonAsync<JsonDocument>(uriSB.ToString());
            }
            catch (HttpRequestException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.TooManyRequests:
                        await Task.Delay(1000);
                        break;
                    default:
                        throw;
                }
            }
        }

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

    internal struct GetQuestionsArgs
    {
        public int Amount { get; set; } = 10;

        public QuestionCategory? Category { get; set; } = null;

        public QuestionDifficulty? Difficulty { get; set; } = null;

        public QuestionType? Type { get; set; } = null;

        public GetQuestionsArgs() { }
    }

    public static TriviaQuestion ParseJson(JsonElement element)
    {
        var category = GetCategory(element.GetProperty("category").GetString());
        var difficulty = GetDifficulty(element.GetProperty("difficulty").GetString());
        var type = GetType(element.GetProperty("type").GetString());

        var question = WebUtility.HtmlDecode(element.GetProperty("question").GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".question is null or undefined"));
        var correctAnswer = WebUtility.HtmlDecode(element.GetProperty("correct_answer").GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".correct_answer is null or undefined"));

        var incorrectAnswers = element.GetProperty("incorrect_answers").EnumerateArray().Select(answer => WebUtility.HtmlDecode(answer.GetString() ?? throw new ArgumentOutOfRangeException(nameof(element), ".incorrect_answers[i] is null or undefined"))).ToList() ?? throw new ArgumentOutOfRangeException(nameof(element), ".incorrect_answers is null or undefined");

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
            case "Entertainment: Books":
                return QuestionCategory.Entertainment_Books;
            case "Entertainment: Film":
                return QuestionCategory.Entertainment_Film;
            case "Entertainment: Music":
                return QuestionCategory.Entertainment_Music;
            case "Entertainment: Musicals &amp; Theatres":
                return QuestionCategory.Entertainment_MusicalsAndTheatres;
            case "Entertainment: Television":
                return QuestionCategory.Entertainment_Television;
            case "Entertainment: Video Games":
                return QuestionCategory.Entertainment_VideoGames;
            case "Entertainment: Board Games":
                return QuestionCategory.Entertainment_BoardGames;
            case "Science &amp; Nature":
                return QuestionCategory.ScienceAndNature;
            case "Science: Computers":
                return QuestionCategory.Science_Computers;
            case "Science: Mathematics":
                return QuestionCategory.Science_Mathematics;
            case "Mythology":
                return QuestionCategory.Mythology;
            case "Sports":
                return QuestionCategory.Sports;
            case "Geography":
                return QuestionCategory.Geography;
            case "History":
                return QuestionCategory.History;
            case "Politics":
                return QuestionCategory.Politics;
            case "Art":
                return QuestionCategory.Art;
            case "Celebrities":
                return QuestionCategory.Celebrities;
            case "Animals":
                return QuestionCategory.Animals;
            case "Vehicles":
                return QuestionCategory.Vehicles;
            case "Entertainment: Comics":
                return QuestionCategory.Entertainment_Comics;
            case "Science: Gadgets":
                return QuestionCategory.Science_Gadgets;
            case "Entertainment: Japanese Anime &amp; Manga":
                return QuestionCategory.Entertainment_JapaneseAnimeAndManga;
            case "Entertainment: Cartoon &amp; Animations":
                return QuestionCategory.Entertainment_CartoonAndAnimations;
        }

        throw new ArgumentOutOfRangeException(nameof(jsonValue), $"Value: \"{ jsonValue }\"");
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

        throw new ArgumentOutOfRangeException(nameof(jsonValue), $"Value: \"{jsonValue}\"");
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