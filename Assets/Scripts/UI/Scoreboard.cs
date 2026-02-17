using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI ScoreboardText;
    public TextMeshProUGUI ScoreboardValue;
    public string ScoreboardValueFormat = "{0:D6}";

    public void SetScoreboardValue(int value)
    {
        ScoreboardValue.text = string.Format(ScoreboardValueFormat, value);
    }
}
