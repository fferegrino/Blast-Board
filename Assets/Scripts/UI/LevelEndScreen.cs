using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class LevelEndScreen : MonoBehaviour
{
    public event Action OnActionButtonClick;

    [Header("Scoreboards")]
    public Scoreboard levelScoreboard;
    public Scoreboard sessionScoreboard;
    public Scoreboard levelDisplay;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    public TextMeshProUGUI screenText;

    void Start()
    {
        actionButton.onClick.AddListener(InnerOnActionButtonClick);
        sessionScoreboard.SetScoreboardText("Session Points");
    }

    public void SetScoreboards(int levelScore, int sessionScore, int level)
    {
        levelScoreboard.SetScoreboardValue(levelScore);
        sessionScoreboard.SetScoreboardValue(sessionScore);
        levelDisplay.SetScoreboardValue(level);
    }

    public void SetActionButtonText(string text)
    {
        if (actionButtonText != null) actionButtonText.text = text;
    }

    public void SetScreenText(string text)
    {
        if (screenText != null) screenText.text = text;
    }

    void InnerOnActionButtonClick()
    {
        OnActionButtonClick?.Invoke();
    }
}
