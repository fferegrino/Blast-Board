using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
public class BoardController : MonoBehaviour
{
    const int CARD_LOCATION_X = -450;
    const int CARD_LOCATION_Y = 450;
    const int CARD_OFFSET = 180;

    const float LOSE_GAME_DELAY = 1f;


    [Header("Board Elements")]
    public GameObject cardButtonsParent;

    public GameObject cardButton;

    public GameObject valueTile;

    public MemoPad memoPad;

    [Header("Scoreboards")]
    public Scoreboard levelScoreboard;
    public Scoreboard sessionScoreboard;
    public LevelDisplay levelDisplay;

    [Header("Screens")]
    public LevelEndScreen levelEndScreen;
    public SettingsUI settingsUI;

    [Header("Sound FX")]
    public AudioClip cardTargetSound;

    public AudioClip cardRevealSound;

    public AudioClip cardExplodeSound;
    public AudioClip gameOverSound;

    public AudioClip gameWonSound;

    public AudioClip gameStartSound;

    public AudioClip cardMarkSound;
    public AudioClip cardUnmarkSound;


    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("UI")]
    public Button settingsButton;
    public Button tutorialButton;
    public Button leaderboardButton;

    [Header("Sizing")]
    public Canvas canvas;

    private Vector3 cardButtonParentPosition;

    private CardButton[] cardButtons;

    private GameSession gameSession;

    /// <summary>Persisted across scene loads (e.g. when opening/closing the tutorial) so game state is restored.</summary>
    private static GameSession s_persistedSession;

    GameState gameState => gameSession?.CurrentGame;

    void Awake()
    {
        levelEndScreen.gameObject.SetActive(true);
        levelEndScreen.OnActionButtonClick += OnLevelEndScreenActionButtonClick;
        memoPad.OnMemoPadClick += OnMemoPadClick;
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        tutorialButton.onClick.AddListener(OnTutorialButtonClick);
        leaderboardButton.onClick.AddListener(OnLeaderboardButtonClick);
    }

    void Start()
    {
        cardButtonParentPosition = cardButtonsParent.transform.position;
        // Use GameSession.DemoSession() for testing
        gameSession = s_persistedSession != null ? s_persistedSession : new GameSession();
        s_persistedSession = null;
        ResetBoard(gameSession.CurrentGame);
        UpdateScoreboards();
    }

    void OnSettingsButtonClick()
    {
        settingsUI.gameObject.SetActive(true);
    }

    void OnTutorialButtonClick()
    {
        s_persistedSession = gameSession;
        SceneManager.LoadScene("TutorialScene");
    }

    void OnLeaderboardButtonClick()
    {
        GameCenterManager.Instance.ShowLeaderboard();
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null && SoundFXManager.Instance != null)
            SoundFXManager.Instance.PlaySound(audioClip, transform);
    }

    void OnMemoPadClick(CellMarks mark)
    {
        if (gameState.Outcome != GameOutcome.InProgress) return;
        if (gameState.TargetedRow < 0 || gameState.TargetedColumn < 0) return;

        int r = gameState.TargetedRow, c = gameState.TargetedColumn;
        if (gameState.HasCellMark(r, c, mark))
        {
            gameState.RemoveCellMark(r, c, mark);
            PlaySound(cardUnmarkSound);
        }
        else{
            gameState.AddCellMark(r, c, mark);
            PlaySound(cardMarkSound);
        }

        RefreshCardFromState(r, c);
    }

    void OnLevelEndScreenActionButtonClick()
    {
        if (gameState.Outcome == GameOutcome.Won)
        {
            gameSession.AdvanceToNextLevel();
            ResetBoard(gameSession.CurrentGame);
        }
        else if (gameState.Outcome == GameOutcome.Lost)
        {
            gameSession.RetryCurrentLevel();
            ResetBoard(gameSession.CurrentGame);
        }
        else
        {
            levelEndScreen.gameObject.SetActive(false);
        }
    }

    void ResetBoard(GameState state)
    {
        foreach (Transform child in cardButtonsParent.transform)
        {
            Destroy(child.gameObject);
        }

        RecreateCards(state);
        CreateValueTiles(state);
        UpdateScoreboards();
        levelEndScreen.gameObject.SetActive(false);
        PlaySound(gameStartSound);
        BackgroundMusicManager.Instance.PlayDefault();
    }

    void UpdateScoreboards()
    {
        if (gameSession == null) return;
        levelScoreboard.SetScoreboardValue(gameState.CurrentPoints);
        sessionScoreboard.SetScoreboardValue(gameSession.SessionPoints);
        levelDisplay.SetLevel(gameSession.CurrentLevel, gameSession.LevelProgress);
    }

    void RecreateCards(GameState state)
    {
        #if UNITY_EDITOR
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Board (total points: {state.PointsToWin}) :");
        for (int row = 0; row < GameState.BoardSize; row++)
        {
            for (int col = 0; col < GameState.BoardSize; col++)
            {
                sb.Append($"{state[row, col]} ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
        #endif

        Debug.Log("Canvas scale factor: " + canvas.scaleFactor);

        var scaledLocationX = CARD_LOCATION_X * canvas.scaleFactor;
        var scaledLocationY = CARD_LOCATION_Y * canvas.scaleFactor;
        var scaledOffset = CARD_OFFSET * canvas.scaleFactor;

        int size = GameState.BoardSize;
        cardButtons = new CardButton[size * size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {

                var positionX = ((scaledLocationX + col * scaledOffset) + cardButtonParentPosition.x);
                var positionY = ((scaledLocationY - row * scaledOffset) + cardButtonParentPosition.y);
                var card = Instantiate(cardButton, new Vector3(positionX, positionY, 0), Quaternion.identity, cardButtonsParent.transform);
                var btn = card.GetComponent<CardButton>();

                card.name = $"CardButton_{row}_{col}";
                btn.SetPosition(row, col);
                btn.SetValue(state[row, col]);
                btn.SetCellState(state.GetCellState(row, col));
                btn.SetMark(
                    state.HasCellMark(row, col, CellMarks.Mark0),
                    state.HasCellMark(row, col, CellMarks.Mark1),
                    state.HasCellMark(row, col, CellMarks.Mark2),
                    state.HasCellMark(row, col, CellMarks.Mark3));
                btn.OnClick += OnCardButtonClick;
                cardButtons[row * size + col] = btn;
            }
        }
    }

    CardButton GetCard(int row, int col)
    {
        int size = GameState.BoardSize;
        if (row < 0 || row >= size || col < 0 || col >= size) return null;
        return cardButtons[row * size + col];
    }

    void RefreshCardFromState(int row, int col)
    {
        var card = GetCard(row, col);
        if (card == null) return;
        card.SetCellState(gameState.GetCellState(row, col));
        card.SetMark(
            gameState.HasCellMark(row, col, CellMarks.Mark0),
            gameState.HasCellMark(row, col, CellMarks.Mark1),
            gameState.HasCellMark(row, col, CellMarks.Mark2),
            gameState.HasCellMark(row, col, CellMarks.Mark3));
    }

    void OnCardButtonClick(CardButton card)
    {
        if (gameState.Outcome != GameOutcome.InProgress)
        {
            Debug.Log($"Game is not in progress. Game state: {gameState.Outcome}");
            return;
        }

        int r = card.Row, c = card.Column;
        bool isAlreadyTargeted = gameState.TargetedRow == r && gameState.TargetedColumn == c;

        if (isAlreadyTargeted)
        {
            var wasRevealed = gameState.TryRevealCell(r, c);
            if (wasRevealed)
            {
                RefreshCardFromState(r, c);
                if (gameState.Outcome == GameOutcome.Lost)
                {
                    if (cardExplodeSound != null && SoundFXManager.Instance != null)
                        SoundFXManager.Instance.PlaySound(cardExplodeSound, transform);
                    StartCoroutine(LoseGame());
                    return;
                }
                if (cardRevealSound != null && SoundFXManager.Instance != null)
                    SoundFXManager.Instance.PlaySound(cardRevealSound, transform);
                if (gameState.Outcome == GameOutcome.Won)
                {
                    UpdateScoreboards();
                    ShowLevelEndScreen(GameOutcome.Won);
                    PlaySound(gameWonSound);
                    return;
                }
            }
            else
            {
                Debug.Log($"Unable to reveal card. Game state: {gameState.Outcome}");
            }
        }
        else
        {
            int prevR = gameState.TargetedRow, prevC = gameState.TargetedColumn;
            if (gameState.SetTargetedCell(r, c))
            {
                if (prevR >= 0 && prevC >= 0)
                    RefreshCardFromState(prevR, prevC);
                RefreshCardFromState(r, c);
                if (cardTargetSound != null && SoundFXManager.Instance != null)
                    SoundFXManager.Instance.PlaySound(cardTargetSound, transform);
            }
        }
        UpdateScoreboards();
    }

    IEnumerator LoseGame()
    {
        BackgroundMusicManager.Instance.Stop();
        UpdateScoreboards();
        yield return StartCoroutine(RevealCardsVisuallyForEffect(gameState.ZeroLocations));
        yield return new WaitForSeconds(0.5f);
        ShowLevelEndScreen(GameOutcome.Lost);
        PlaySound(gameOverSound);
    }

    /// <summary>
    /// Visual effect only: animates cards to look revealed without changing GameState.
    /// Use for lose animation; positions (e.g. gameState.ZeroLocations) come from state, but we never write back.
    /// </summary>
    IEnumerator RevealCardsVisuallyForEffect(IReadOnlyList<Tuple<int, int>> positions)
    {
        var delay = LOSE_GAME_DELAY / positions.Count;
        foreach (var pos in positions)
        {
            var card = GetCard(pos.Item1, pos.Item2);
            if (card != null)
            {
                yield return new WaitForSeconds(delay);
                card.SetCellState(CellState.Revealed);
                if (cardRevealSound != null && SoundFXManager.Instance != null)
                    if (card.Value == 0)
                        SoundFXManager.Instance.PlaySound(cardExplodeSound, transform);
                    else
                        SoundFXManager.Instance.PlaySound(cardRevealSound, transform);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    void ReportGameCenterScoreOnLevelWin()
    {
        if (GameCenterManager.Instance == null)
        {
            return;
        }

        long totalScore = gameSession.SessionPoints + gameState.CurrentPoints;
        GameCenterManager.Instance.ReportScore(totalScore);
        int level = gameSession.CurrentLevel;
        if (level >= 1)
        {
            GameCenterManager.Instance.ReportAchievement(GameCenterManager.Instance.achievementFirstWin, 100.0);
        }
    }

    void ShowLevelEndScreen(GameOutcome outcome)
    {
        levelEndScreen.SetScoreboards(gameState.CurrentPoints, gameSession.SessionPoints, gameSession.CurrentLevel, gameSession.LevelProgress);
        if (outcome == GameOutcome.Won)
        {
            levelEndScreen.SetWon();
            ReportGameCenterScoreOnLevelWin();
        }
        else if (outcome == GameOutcome.Lost)
        {
            levelEndScreen.SetLost();
        }
        levelEndScreen.gameObject.SetActive(true);
    }

    void CreateValueTiles(GameState state)
    {
        var scaledLocationX = CARD_LOCATION_X * canvas.scaleFactor;
        var scaledLocationY = CARD_LOCATION_Y * canvas.scaleFactor;
        var scaledOffset = CARD_OFFSET * canvas.scaleFactor;

        int size = GameState.BoardSize;
        for (int i = 0; i < size; i++)
        {
            var tile = Instantiate(valueTile,
                new Vector3(
                    (scaledLocationX + i * scaledOffset) + cardButtonParentPosition.x,
                    (scaledLocationY - size * scaledOffset) + cardButtonParentPosition.y,
                    0
                ),
                Quaternion.identity, cardButtonsParent.transform);
            tile.name = $"ColumnValue_{i}";
            tile.GetComponent<ValuesTile>().SetValues(state.ColumnBombs[i], state.ColumnSumValues[i]);
        }

        for (int i = 0; i < size; i++)
        {
            var tile = Instantiate(valueTile,
                new Vector3(
                    (scaledLocationX + size * scaledOffset) + cardButtonParentPosition.x,
                    (scaledLocationY - i * scaledOffset) + cardButtonParentPosition.y,
                    0
                ),
                Quaternion.identity, cardButtonsParent.transform);
            tile.name = $"RowValue_{i}";
            tile.GetComponent<ValuesTile>().SetValues(state.RowBombs[i], state.RowSumValues[i]);
        }
    }
}
