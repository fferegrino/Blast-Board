using UnityEngine;

public class BoardController : MonoBehaviour
{
    const int CARD_LOCATION_X = -450;
    const int CARD_LOCATION_Y = 450;
    const int CARD_OFFSET = 180;

    public GameObject cardButtonsParent;

    public GameObject cardButton;

    public GameObject valueTile;

    private Vector3 cardButtonParentPosition;

    private CardButton[] cardButtons;

    private GameState gameState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardButtonParentPosition = cardButtonsParent.transform.position;
        gameState = new GameState(
            new RawBoard(new int[,]
            {
                { 1, 0, 0, 0, 1 },
                { 0, 2, 0, 0, 2 },
                { 0, 3, 3, 0, 3 },
                { 0, 0, 3, 0, 3 },
                { 0, 0, 0, 0, 3 }
            }
        ));
        ResetBoard(gameState);
    }

    void ResetBoard(GameState gameState)
    {
        foreach (Transform child in cardButtonsParent.transform)
        {
            Destroy(child.gameObject);
        }

        RecreateCards(gameState);
        CreateValueTiles(gameState);
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
        var wasRevealed = gameState.TryRevealCell(card.Row, card.Column);
        if (wasRevealed)
            RefreshCardFromState(card.Row, card.Column);
        else
            Debug.Log($"Unable to reveal card. Game state: {gameState.Outcome}");
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
