using System;

public static class VoltorbDifficultyModel
{
    // SR in [0, 400] -> d in [1, 21], clamp as you like
    public static int DifficultyFromSR(float sr)
    {
        sr = Math.Clamp(sr, 0f, 400f);
        return Math.Clamp((int)(sr / 20f) + 1, 1, 20);
    }

    public static VoltorbDifficulty GetDifficultyParams(int d, int boardSize = 5)
    {
        d = Math.Clamp(d, 1, 20);
        int totalTiles = boardSize * boardSize;

        int voltorbs = Math.Clamp(4 + (int)(0.3f * d), 3, totalTiles - 4);
        int highTiles = Math.Clamp(6 + (int)(0.4f * d), 4, totalTiles - voltorbs);

        float threeRatio = Math.Clamp(0.25f + 0.025f * d, 0f, 1f); // 25% -> 75%
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
