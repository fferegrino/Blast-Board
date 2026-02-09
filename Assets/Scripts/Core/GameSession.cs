using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks level progression and session points across multiple rounds.
/// When the player wins a level, their points are added to the session and the level increases.
/// </summary>
public class GameSession
{
    private static readonly Random Rng = new Random();

    /// <summary>Current level (1-based).</summary>
    public int Level { get; private set; }

    /// <summary>Total points accumulated this session (sum of CurrentPoints at each level win).</summary>
    public int SessionPoints { get; private set; }

    /// <summary>The current level's board and round state.</summary>
    public GameState CurrentGame { get; private set; }

    public GameSession()
    {
        Level = 1;
        SessionPoints = 0;
        CurrentGame = CreateGameForLevel(1);
    }

    public static GameSession DemoSession()
    {
        var session = new GameSession
        {
            Level = 1,
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
        Level++;
        CurrentGame = CreateGameForLevel(Level);
        return true;
    }

    /// <summary>
    /// Starts a new round for the current level (e.g. after a loss, to retry).
    /// Session points and level are unchanged.
    /// </summary>
    public void RetryCurrentLevel()
    {
        CurrentGame = CreateGameForLevel(Level);
    }

    /// <summary>
    /// Starts a new session from level 1 with zero session points.
    /// </summary>
    public void StartNewSession()
    {
        Level = 1;
        SessionPoints = 0;
        CurrentGame = CreateGameForLevel(1);
    }

    private static GameState CreateGameForLevel(int level)
    {
        var definitions = GetLevelDefinitions(level);
        var definition = definitions[Rng.Next(definitions.Count)];
        var board = new RawBoard(definition);
        return new GameState(board);
    }

    private static List<LevelDefinition> GetLevelDefinitions(int level)
    {
        if (LevelDefinition.LEVEL_DEFINITIONS.TryGetValue(level, out var list))
        {
            return list;
        }

        int maxLevel = LevelDefinition.LEVEL_DEFINITIONS.Keys.Max();
        return LevelDefinition.LEVEL_DEFINITIONS[Math.Min(level, maxLevel)];
    }
}
