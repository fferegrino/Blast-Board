using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class LevelEndScreen : MonoBehaviour
{
    public event Action OnActionButtonClick;
    private Button actionButton;
    private TextMeshProUGUI actionButtonText;

    private TextMeshProUGUI screenText;

    void Awake()
    {
        var allChildren = GetComponentsInChildren<Transform>(true);
        actionButton = allChildren.First(child => child.name == "ActionButton").GetComponent<Button>();
        actionButtonText = allChildren.First(child => child.name == "Text").GetComponent<TextMeshProUGUI>();
        screenText = allChildren.First(child => child.name == "ScreenText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        actionButton.onClick.AddListener(InnerOnActionButtonClick);
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
