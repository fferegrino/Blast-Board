using System;

/// <summary>
/// Tracks level progression and session points across multiple rounds.
/// When the player wins a level, their points are added to the session and the level increases.
/// </summary>
public class GameSession
{

    const float SkillRatingPerLevel = 10f;

    public int CurrentLevel => VoltorbDifficultyModel.DifficultyFromSR(SkillRating);


    /// <summary>Total points accumulated this session (sum of CurrentPoints at each level win).</summary>
    public int SessionPoints { get; private set; }

    /// <summary>The current level's board and round state.</summary>
    public GameState CurrentGame { get; private set; }

    public int BoardSize = 5;
    public float SkillRating = 0f; // 0-400

    public float LevelProgress
    {
        get
        {
            float levelStartSR = (CurrentLevel - 1) * SkillRatingPerLevel;
            float levelEndSR = CurrentLevel * SkillRatingPerLevel;

            float levelProgress = InverseLerp(levelStartSR, levelEndSR, SkillRating);
            return levelProgress;
        }
    }

    public int UserFacingLevel {
        get {
            return CurrentLevel + (int)(LevelProgress * SkillRatingPerLevel);
        }
    }


    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
        {
            return (value - a) / (b - a);
        }
        else
        {
            return 0f;
        }
    }
    private VoltorbBoardGenerator generator = new VoltorbBoardGenerator();

    private GameState NewGame()
    {
        int d = VoltorbDifficultyModel.DifficultyFromSR(SkillRating);
        var diff = VoltorbDifficultyModel.GetDifficultyParams(d, BoardSize);

        // Debug.Log($"NewGame: SkillRating = {SkillRating} - Difficulty = {d} - LevelProgress = {LevelProgress}");
        VoltorbBoard currentBoard = generator.GenerateBoard(BoardSize, diff);
        return new GameState(new RawBoard(currentBoard));
    }

    public GameSession()
    {
        SessionPoints = 0;
        CurrentGame = NewGame();
    }

    public static GameSession DemoSession()
    {
        var session = new GameSession
        {
            SessionPoints = 0,
        };
        session.CurrentGame = new GameState(
            new RawBoard(new int[,]
            {
                { 1, 0, 0, 0, 1 },
                { 0, 2, 0, 0, 2 },
                { 0, 3, 3, 0, 3 },
                { 0, 0, 3, 0, 3 },
                { 0, 0, 0, 0, 3 }
            }
        ));
        return session;
    }


    public void OnRoundWon(float multiplier, int safeTilesFlipped)
    {
        // Reward wins; scale a bit by multiplier
        SkillRating += 5f + Math.Max(0f, multiplier - 1f) * 2f;
        SkillRating = Math.Clamp(SkillRating, 0f, 400f);
    }

    public void OnHitVoltorb(int safeTilesFlipped)
    {
        // Penalize more if you died early
        float factor = 1f - (safeTilesFlipped / (float)(BoardSize * BoardSize));
        SkillRating -= 7f * factor;
        SkillRating = Math.Clamp(SkillRating, 0f, 400f);
    }

    /// <summary>
    /// Call when the current round is won. Adds this round's points to SessionPoints, increments Level,
    /// and starts a new round for the next level. Returns true if advanced.
    /// </summary>
    public bool AdvanceToNextLevel()
    {
        if (CurrentGame.Outcome != GameOutcome.Won)
        {
            return false;
        }

        SessionPoints += CurrentGame.CurrentPoints;
        OnRoundWon(1, CurrentGame.TilesRevealed);
        // Level++;
        CurrentGame = NewGame();
        return true;
    }

    /// <summary>
    /// Starts a new round for the current level (e.g. after a loss, to retry).
    /// Session points and level are unchanged.
    /// </summary>
    public void RetryCurrentLevel()
    {
        OnHitVoltorb(CurrentGame.TilesRevealed);
        CurrentGame = NewGame();
    }
}
