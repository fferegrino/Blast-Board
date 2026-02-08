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

    void RecreateCards(GameState gameState)
    {
        cardButtons = new CardButton[25];
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                var card = Instantiate(cardButton,
                    new Vector3(
                        (CARD_LOCATION_X + col * CARD_OFFSET) + cardButtonParentPosition.x,
                        (CARD_LOCATION_Y - row * CARD_OFFSET) + cardButtonParentPosition.y,
                        0
                    ), 
                    Quaternion.identity, cardButtonsParent.transform);
                var backingClass = card.GetComponent<CardButton>();

                card.name = $"CardButton_{row}_{col}";
                backingClass.SetPosition(row, col);
                backingClass.SetValue(gameState[row, col]);
                backingClass.OnClick += OnCardButtonClick;
                cardButtons[row * 5 + col] = backingClass;
            }
        }
    }

    void OnCardButtonClick(CardButton cardButton)
    {
        var wasAbleToReveal = gameState.TryRevealCell(cardButton.Row, cardButton.Column);
        if (wasAbleToReveal)
        {
            cardButton.SetCellState(CellState.Revealed);
        }
        else
        {
            Debug.Log($"Unable to reveal card. Game state: {gameState.Outcome}");
        }
    }

    void CreateValueTiles(GameState gameState)
    {
        // Column values
        for (int i = 0; i < 5; i++)
        {
            var tile = Instantiate(valueTile,
                new Vector3(
                    (CARD_LOCATION_X + i * CARD_OFFSET) + cardButtonParentPosition.x,
                    (CARD_LOCATION_Y - 5 * CARD_OFFSET) + cardButtonParentPosition.y,
                    0
                ),
                Quaternion.identity, cardButtonsParent.transform);
            tile.name = $"ColumnValue_{i}";
            tile.GetComponent<ValuesTile>().SetValues(gameState.ColumnBombs[i], gameState.ColumnSumValues[i]);
        }

        // Row values
        for (int i = 0; i < 5; i++)
        {
            var tile = Instantiate(valueTile,
                new Vector3(
                    (CARD_LOCATION_X + 5 * CARD_OFFSET) + cardButtonParentPosition.x,
                    (CARD_LOCATION_Y - i * CARD_OFFSET) + cardButtonParentPosition.y,
                    0
                ), Quaternion.identity, cardButtonsParent.transform);
                tile.name = $"RowValue_{i}";
            tile.GetComponent<ValuesTile>().SetValues(gameState.RowBombs[i], gameState.RowSumValues[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
