using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.Localization;

public class LevelEndScreen : MonoBehaviour
{
    public event Action OnActionButtonClick;

    [Header("Scoreboards")]
    public Scoreboard levelScoreboard;
    public Scoreboard sessionScoreboard;
    public Scoreboard levelDisplay;

    [Header("Texts")]
    public LocalizedString retryActionText;
    public LocalizedString nextActionText;
    public LocalizedString wonScreenText;
    public LocalizedString lostScreenText;

    [Header("Buttons")]
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    public TextMeshProUGUI screenText;

    private string innerRetryActionText;
    private string innerNextActionText;
    private string innerWonScreenText;
    private string innerLostScreenText;

    void Awake()
    {
        retryActionText.StringChanged += OnRetryActionTextChanged;
        nextActionText.StringChanged += OnNextActionTextChanged;
        wonScreenText.StringChanged += OnWonScreenTextChanged;
        lostScreenText.StringChanged += OnLostScreenTextChanged;
    }

    void OnRetryActionTextChanged(string value)
    {
        innerRetryActionText = value;
    }

    void OnNextActionTextChanged(string value)
    {
        innerNextActionText = value;
    }

    void OnWonScreenTextChanged(string value)
    {
        innerWonScreenText = value;
    }

    void OnLostScreenTextChanged(string value)
    {
        innerLostScreenText=value;
    }

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

    public void SetLost() {
        actionButtonText.text = innerRetryActionText;
        screenText.text = innerLostScreenText;
    }

    public void SetWon() {
        actionButtonText.text = innerNextActionText;
        screenText.text = innerWonScreenText;
    }

    void InnerOnActionButtonClick()
    {
        OnActionButtonClick?.Invoke();
    }
}
