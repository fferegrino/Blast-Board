using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI ScoreboardText;
    public TextMeshProUGUI ScoreboardValue;
    public string ScoreboardValueFormat = "{0:D6}";

    public void SetScoreboardValue(int value)
    {
        ScoreboardValue.text = string.Format(ScoreboardValueFormat, value);
    }

    public void SetScoreboardText(string text)
    {
        ScoreboardText.text = text;
    }
}
