using UnityEngine;
using System.Collections;

public class BoardController : MonoBehaviour
{
    const int CARD_LOCATION_X = -450;
    const int CARD_LOCATION_Y = 450;
    const int CARD_OFFSET = 180;


    [Header("Board Elements")]
    public GameObject cardButtonsParent;

    public GameObject cardButton;

    public GameObject valueTile;

    public MemoPad memoPad;

    [Header("Scoreboards")]
    public Scoreboard levelScoreboard;
    public Scoreboard sessionScoreboard;
    public Scoreboard levelDisplay;

    [Header("Screens")]
    public LevelEndScreen levelEndScreen;

    private Vector3 cardButtonParentPosition;

    private CardButton[] cardButtons;

    private GameSession gameSession;

    GameState gameState => gameSession?.CurrentGame;

    void Start()
    {
        cardButtonParentPosition = cardButtonsParent.transform.position;
        gameSession = GameSession.DemoSession();
        ResetBoard(gameSession.CurrentGame);
        UpdateScoreboards();
        levelEndScreen.OnActionButtonClick += OnLevelEndScreenActionButtonClick;
        memoPad.OnMemoPadClick += OnMemoPadClick;
    }

    void OnMemoPadClick(CellMarks mark)
    {
        Debug.Log($"OnMemoPadClick: {mark}");
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
    }

    void UpdateScoreboards()
    {
        if (gameSession == null) return;
        levelScoreboard.SetScoreboardValue(gameState.CurrentPoints);
        sessionScoreboard.SetScoreboardValue(gameSession.SessionPoints);
        levelDisplay.SetScoreboardValue(gameSession.Level);
    }

    void RecreateCards(GameState state)
    {
        int size = GameState.BoardSize;
        cardButtons = new CardButton[size * size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                var card = Instantiate(cardButton,
                    new Vector3(
                        (CARD_LOCATION_X + col * CARD_OFFSET) + cardButtonParentPosition.x,
                        (CARD_LOCATION_Y - row * CARD_OFFSET) + cardButtonParentPosition.y,
                        0
                    ),
                    Quaternion.identity, cardButtonsParent.transform);
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
                if (gameState.Outcome == GameOutcome.Won)
                {
                    UpdateScoreboards();
                    ShowLevelEndScreen(GameOutcome.Won);
                    return;
                }
                else if (gameState.Outcome == GameOutcome.Lost)
                {
                    StartCoroutine(LoseGame());
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
            }
        }
        UpdateScoreboards();
    }

    IEnumerator LoseGame()
    {
        UpdateScoreboards();
        foreach (var zeroLocation in gameState.ZeroLocations)
        {
            var card = GetCard(zeroLocation.Item1, zeroLocation.Item2);
            if (card != null)
            {
                yield return new WaitForSeconds(0.3f);
                card.SetCellState(CellState.Revealed);
            }
        }
        yield return new WaitForSeconds(0.5f);
        ShowLevelEndScreen(GameOutcome.Lost);
    }

    void ShowLevelEndScreen(GameOutcome outcome)
    {
        if (outcome == GameOutcome.Won)
        {
            levelEndScreen.SetActionButtonText("Next Level");
            levelEndScreen.SetScreenText("You won the level!");
        }
        else if (outcome == GameOutcome.Lost)
        {
            levelEndScreen.SetActionButtonText("Restart Level");
            levelEndScreen.SetScreenText("You lost the level!");
        }
        levelEndScreen.gameObject.SetActive(true);
    }

    void CreateValueTiles(GameState state)
    {
        int size = GameState.BoardSize;
        for (int i = 0; i < size; i++)
        {
            var tile = Instantiate(valueTile,
                new Vector3(
                    (CARD_LOCATION_X + i * CARD_OFFSET) + cardButtonParentPosition.x,
                    (CARD_LOCATION_Y - size * CARD_OFFSET) + cardButtonParentPosition.y,
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
                    (CARD_LOCATION_X + size * CARD_OFFSET) + cardButtonParentPosition.x,
                    (CARD_LOCATION_Y - i * CARD_OFFSET) + cardButtonParentPosition.y,
                    0
                ),
                Quaternion.identity, cardButtonsParent.transform);
            tile.name = $"RowValue_{i}";
            tile.GetComponent<ValuesTile>().SetValues(state.RowBombs[i], state.RowSumValues[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
