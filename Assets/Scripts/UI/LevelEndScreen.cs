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
    private Button actionButton;
    public TextMeshProUGUI actionButtonText;

    private TextMeshProUGUI screenText;

    void Awake()
    {
        var allChildren = GetComponentsInChildren<Transform>(true);
        actionButton = allChildren.First(child => child.name == "ActionButton").GetComponent<Button>();
        screenText = allChildren.First(child => child.name == "ScreenText").GetComponent<TextMeshProUGUI>();
        sessionScoreboard.SetScoreboardText("Points in this session");
    }

    void Start()
    {
        actionButton.onClick.AddListener(InnerOnActionButtonClick);
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
