using System;

/// <summary>
/// Tracks level progression and session points across multiple rounds.
/// When the player wins a level, their points are added to the session and the level increases.
/// </summary>
public class GameSession
{
    const float MinSkillLevelIncrement = 1f;
    const float MaxRewardSkillRating = 5f;

    /// <summary>Total points accumulated this session (sum of CurrentPoints at each level win).</summary>
    public int SessionPoints { get; private set; }

    /// <summary>The current level's board and round state.</summary>
    public GameState CurrentGame { get; private set; }

    public int BoardSize = 5;
    private float skillRating = VoltorbDifficultyModel.MinSkillRating;

    /// <summary>Display level (1–40); from VoltorbDifficultyModel so it increases monotonically with SR.</summary>
    public int CurrentLevel => VoltorbDifficultyModel.LevelFromSR(skillRating);

    private VoltorbBoardGenerator generator = new VoltorbBoardGenerator();

    private GameState NewGame()
    {
        int d = VoltorbDifficultyModel.DifficultyFromSR(skillRating);
        var diff = VoltorbDifficultyModel.GetDifficultyParams(d, BoardSize);

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


    /// <summary>Returns the skill rating change if a win reward were applied (does not modify SkillRating).</summary>
    public float GetSkillRatingDeltaIfWin() =>
        MinSkillLevelIncrement + (float)(CurrentGame?.RevealedToValuableRatio ?? 0) * MaxRewardSkillRating;

    /// <summary>Returns the skill rating change if a bomb penalty were applied (does not modify SkillRating).</summary>
    public float GetSkillRatingDeltaIfLose() =>
        -(MinSkillLevelIncrement + (float)(CurrentGame?.SafeProgressRatio ?? 0) * MaxRewardSkillRating);

    /// <summary>Returns the level that would result from the given skill rating (delegates to VoltorbDifficultyModel).</summary>
    public int GetLevelFromSkillRating(float sr) =>
        VoltorbDifficultyModel.LevelFromSR(Math.Clamp(sr, VoltorbDifficultyModel.MinSkillRating, VoltorbDifficultyModel.MaxSkillRating));

    /// <summary>Returns the level that would result if the current round were won (does not modify state).</summary>
    public int GetLevelIfWin() =>
        GetLevelFromSkillRating(Math.Clamp(skillRating + GetSkillRatingDeltaIfWin(), VoltorbDifficultyModel.MinSkillRating, VoltorbDifficultyModel.MaxSkillRating));

    /// <summary>Returns the level that would result if the current round were lost (does not modify state).</summary>
    public int GetLevelIfLose() =>
        GetLevelFromSkillRating(Math.Clamp(skillRating + GetSkillRatingDeltaIfLose(), VoltorbDifficultyModel.MinSkillRating, VoltorbDifficultyModel.MaxSkillRating));

    /// <summary>Returns the level difference (level after win minus current level). Positive when winning would level up.</summary>
    public int GetLevelDeltaIfWin() => GetLevelIfWin() - CurrentLevel;

    /// <summary>Returns the level difference (level after lose minus current level). Negative when losing would level down.</summary>
    public int GetLevelDeltaIfLose() => GetLevelIfLose() - CurrentLevel;

    /// <summary>Applies skill rating reward for winning; scaled by how efficiently valuable tiles were revealed.</summary>
    public void ApplyWinReward()
    {
        // Reward wins; scale a bit by multiplier
        skillRating += MinSkillLevelIncrement + (float)CurrentGame.RevealedToValuableRatio * MaxRewardSkillRating;
        skillRating = Math.Clamp(skillRating, VoltorbDifficultyModel.MinSkillRating, VoltorbDifficultyModel.MaxSkillRating);
    }

    /// <summary>Applies skill rating penalty for hitting a bomb; penalizes more when dying early (fewer tiles revealed).</summary>
    public void ApplyBombPenalty()
    {
        // Penalize more if you died early
        skillRating -= (MinSkillLevelIncrement + (float)CurrentGame.SafeProgressRatio * MaxRewardSkillRating);
        skillRating = Math.Clamp(skillRating, VoltorbDifficultyModel.MinSkillRating, VoltorbDifficultyModel.MaxSkillRating);
    }

    /// <summary>
    /// Call when the current round is won. Adds this round's points to SessionPoints, increments Level,
    /// and starts a new round for the next level. Returns true if advanced.
    /// </summary>
    public bool WinLevel()
    {
        if (CurrentGame.Outcome != GameOutcome.Won)
        {
            return false;
        }

        SessionPoints += CurrentGame.CurrentPoints;
        ApplyWinReward();
        CurrentGame = NewGame();
        return true;
    }

    /// <summary>
    /// Starts a new round for the current level (e.g. after a loss, to retry).
    /// Session points and level are unchanged.
    /// </summary>
    public bool LoseLevel()
    {
        if (CurrentGame.Outcome != GameOutcome.Lost)
        {
            return false;
        }

        SessionPoints += CurrentGame.CurrentPoints;
        ApplyBombPenalty();
        CurrentGame = NewGame();
        return true;
    }
}
