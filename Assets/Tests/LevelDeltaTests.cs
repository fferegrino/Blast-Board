using System.Reflection;
using NUnit.Framework;

/// <summary>
/// Tests that the level delta calculation never shows the wrong sign (e.g. negative on win).
/// Level is now defined solely by VoltorbDifficultyModel.LevelFromSR (monotonic in SR).
/// </summary>
public class LevelDeltaTests
{
    /// <summary>
    /// Creates a GameState that is in Won outcome (minimal board, reveal until win).
    /// </summary>
    static GameState CreateWonGameState()
    {
        // PointsToWin = 1*2*2*1 * 1s... = 4. Reveal (0,0), (0,1), (0,3) to win.
        var board = new RawBoard(new int[,]
        {
            { 1, 2, 0, 2, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var state = new GameState(board);
        state.TryRevealCell(0, 0);
        state.TryRevealCell(0, 1);
        state.TryRevealCell(0, 3);
        Assert.That(state.Outcome, Is.EqualTo(GameOutcome.Won), "Setup: game must be won");
        return state;
    }

    /// <summary>
    /// Sets the private skillRating field on the session (for boundary testing).
    /// </summary>
    static void SetSkillRating(GameSession session, float value)
    {
        var field = typeof(GameSession).GetField("skillRating",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(field, Is.Not.Null, "skillRating field must exist");
        field.SetValue(session, value);
    }

    /// <summary>
    /// Sets the private CurrentGame property on the session so we can force a won state.
    /// </summary>
    static void SetCurrentGame(GameSession session, GameState game)
    {
        var setter = typeof(GameSession).GetProperty("CurrentGame")?.GetSetMethod(nonPublic: true);
        Assert.That(setter, Is.Not.Null, "CurrentGame setter must exist");
        setter.Invoke(session, new object[] { game });
    }

    [Test]
    public void LevelDeltaIfWin_WhenSkillRatingJustBelowDifficultyBoundary_ShouldBeNonNegative()
    {
        // SR just below 20 used to cause a level drop (old formula). LevelFromSR is now monotonic.
        var session = new GameSession();
        SetSkillRating(session, 19.9f);
        SetCurrentGame(session, CreateWonGameState());

        int levelDelta = session.GetLevelDeltaIfWin();

        Assert.That(levelDelta, Is.GreaterThanOrEqualTo(0),
            "On a win, level delta must never be negative (bug: every 3rd game shows opposite sign when crossing SR boundary).");
    }

    [Test]
    public void LevelDeltaSign_IsCorrect_ForWinAndLose_AtVariousSkillRatings()
    {
        var session = new GameSession();
        var wonState = CreateWonGameState();

        // Test at several SR values: mid-band, just below boundary, just above boundary.
        float[] winTestRatings = { 5f, 15f, 19.9f, 25f, 39.9f };
        foreach (float sr in winTestRatings)
        {
            SetSkillRating(session, sr);
            SetCurrentGame(session, CreateWonGameState());
            int deltaWin = session.GetLevelDeltaIfWin();
            Assert.That(deltaWin, Is.GreaterThanOrEqualTo(0),
                $"GetLevelDeltaIfWin() at skillRating={sr} must be >= 0 (got {deltaWin}).");
        }

        // For lose we need a lost state. Create a board with one bomb, reveal it.
        var loseBoard = new RawBoard(new int[,]
        {
            { 0, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 }
        });
        var lostState = new GameState(loseBoard);
        lostState.TryRevealCell(0, 0);
        Assert.That(lostState.Outcome, Is.EqualTo(GameOutcome.Lost));

        float[] loseTestRatings = { 5f, 15f, 19.9f, 25f };
        foreach (float sr in loseTestRatings)
        {
            SetSkillRating(session, sr);
            SetCurrentGame(session, lostState);
            int deltaLose = session.GetLevelDeltaIfLose();
            Assert.That(deltaLose, Is.LessThanOrEqualTo(0),
                $"GetLevelDeltaIfLose() at skillRating={sr} must be <= 0 (got {deltaLose}).");
        }
    }

    [Test]
    public void LevelDelta_OverThreeConsecutiveWins_AlwaysNonNegative()
    {
        // Reproduce "every 3rd game" by simulating games 1, 2, 3 at SR 0, 10, 19.9.
        // The 3rd game (SR just below 20) hits the difficulty boundary bug: delta flips negative on win.
        var session = new GameSession();
        for (int game = 0; game < 3; game++)
        {
            float sr = game switch { 0 => 0f, 1 => 10f, _ => 19.9f };
            SetSkillRating(session, sr);
            SetCurrentGame(session, CreateWonGameState());
            int delta = session.GetLevelDeltaIfWin();
            Assert.That(delta, Is.GreaterThanOrEqualTo(0),
                $"Game {game + 1}/3 at skillRating={sr}: level delta on win must be >= 0 (got {delta}).");
        }
    }
}
