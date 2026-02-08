using UnityEngine;
using TMPro;

public class ValuesTile : MonoBehaviour
{
    public TextMeshProUGUI bombsText;
    public TextMeshProUGUI pointsText;

    public void SetValues(int bombs, int points)
    {
        SetBombs(bombs);
        SetPoints(points);
    }

    public void SetBombs(int bombs)
    {
        bombsText.text = bombs.ToString();
    }
    
    public void SetPoints(int points)
    {
        pointsText.text = points.ToString();
    }
}
