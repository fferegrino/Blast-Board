using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelDisplay : MonoBehaviour
{
    public TextMeshProUGUI ScoreboardText;
    public TextMeshProUGUI ScoreboardValue;
    public TextMeshProUGUI LevelProgressText;


    public void SetLevel(int level, float progress)
    {
        SetProgress(progress);
        SetScoreboardValue(level);
    }
    public void SetProgress(float progress)
    {
        LevelProgressText.text = string.Format("{0:.0}", progress);
    }

    public void SetScoreboardValue(int value)
    {
        ScoreboardValue.text = string.Format("{0:D2}", value);
    }

    public void SetScoreboardText(string text)
    {
        ScoreboardText.text = text;
    }
}
