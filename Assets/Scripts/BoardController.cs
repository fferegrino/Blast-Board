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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardButtonParentPosition = cardButtonsParent.transform.position;
        ResetBoard();
    }

    void ResetBoard()
    {
        foreach (Transform child in cardButtonsParent.transform)
        {
            Debug.Log($"Destroying {child.transform.position}");
            Destroy(child.gameObject);
        }

        RecreateCards();
        CreateValueTiles();
    }

    void RecreateCards()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var card = Instantiate(cardButton,
                    new Vector3(
                        (CARD_LOCATION_X + i * CARD_OFFSET) + cardButtonParentPosition.x,
                        (CARD_LOCATION_Y - j * CARD_OFFSET) + cardButtonParentPosition.y,
                        0
                    ), 
                    Quaternion.identity, cardButtonsParent.transform);
                card.name = $"CardButton_{i}_{j}";
            }
        }
    }

    void CreateValueTiles()
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
