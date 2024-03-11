// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace TriviaService;

internal enum QuestionCategory
{
    GeneralKnowledge = 9,
    Entertainment_Books,
    Entertainment_Film,
    Entertainment_Music,
    Entertainment_MusicalsAndTheatres,
    Entertainment_Television,
    Entertainment_VideoGames,
    Entertainment_BoardGames,
    ScienceAndNature,
    Science_Computers,
    Science_Mathematics,
    Mythology,
    Sports,
    Geography,
    History,
    Politics,
    Art,
    Celebrities,
    Animals,
    Vehicles,
    Entertainment_Comics,
    Science_Gadgets,
    Entertainment_JapaneseAnimeAndManga,
    Entertainment_CartoonAndAnimations,
}

internal enum QuestionDifficulty
{
    Easy,
    Medium,
    Hard
}

internal enum QuestionType
{
    MultipleChoice,
    TrueFalse,
}