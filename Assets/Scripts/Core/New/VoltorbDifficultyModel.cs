using System;

/// <summary>
/// Single source of truth for skill rating (SR), display level, and board difficulty.
/// SR is in [0, 400]. Display level is 1–400 (1 SR per level, strictly increasing).
/// Difficulty for board generation is 1–20 (20 SR per step, used by GetDifficultyParams).
/// </summary>
public static class VoltorbDifficultyModel
{
    public const float MinSkillRating = 0f;
    public const float MaxSkillRating = 100f;

    /// <summary>Maximum display level (400 levels, 1 SR each).</summary>
    public const int MaxLevel = 100;

    /// <summary>SR per display level; 1 SR = 1 level for even distribution over [0, 400].</summary>
    public const float SkillRatingPerLevel = 1f;

    /// <summary>SR span per difficulty step; difficulty = 1 + (int)(sr / 20).</summary>
    const float SkillRatingPerDifficultyStep = 20f;

    /// <summary>Display level from skill rating (1–400). Strictly increasing with SR; use for UI and deltas.</summary>
    public static int LevelFromSR(float sr)
    {
        sr = Math.Clamp(sr, MinSkillRating, MaxSkillRating);
        return Math.Clamp(1 + (int)(sr / SkillRatingPerLevel), 1, MaxLevel);
    }

    /// <summary>Board difficulty from skill rating (1–20). Used only for GetDifficultyParams.</summary>
    public static int DifficultyFromSR(float sr)
    {
        sr = Math.Clamp(sr, MinSkillRating, MaxSkillRating);
        return Math.Clamp((int)(sr / SkillRatingPerDifficultyStep) + 1, 1, 20);
    }

    public static VoltorbDifficulty GetDifficultyParams(int d, int boardSize = 5)
    {
        d = Math.Clamp(d, 1, (int)SkillRatingPerDifficultyStep);
        int totalTiles = boardSize * boardSize;

        int voltorbs = Math.Clamp(4 + (int)(0.3f * d), 3, totalTiles - 4);

        // Controls how many high tiles are in the board,
        int highTiles = Math.Clamp(3 + (int)(0.6f * d), 3, totalTiles - voltorbs);
        // int highTiles = Math.Clamp(6 + (int)(0.4f * d), 4, totalTiles - voltorbs);

        // Controls the ratio of threes to the total number of high tiles
        // Start with almost all 2s, only later introduce 3s meaningfully
        float threeRatio = Math.Clamp(0.1f + 0.02f * d, 0f, 1f);  // d=1 -> 12%, d=20 -> 50% 3s
        // float threeRatio = Math.Clamp(0.25f + 0.025f * d, 0f, 1f); // 25% -> 75%

        int threes = Math.Clamp((int)Math.Round (highTiles * threeRatio), 0, highTiles);
        int twos = highTiles - threes;

        int maxFreeTotal = Math.Max(1, 8 - d / 2); // gradually stricter
        int maxFreeLine = Math.Max(1, 3 - d / 5);

        return new VoltorbDifficulty
        {
            Difficulty = d,
            Voltorbs = voltorbs,
            HighTiles = highTiles,
            Threes = threes,
            Twos = twos,
            MaxFreeHighTotal = maxFreeTotal,
            MaxFreeHighLine = maxFreeLine
        };
    }
}
