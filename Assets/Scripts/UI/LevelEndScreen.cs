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

    void Start()
    {

        var allChildren = GetComponentsInChildren<Transform>();
        actionButton = allChildren.First(child => child.name == "ActionButton").GetComponent<Button>();
        actionButtonText = allChildren.First(child => child.name == "Text").GetComponent<TextMeshProUGUI>();
        screenText = allChildren.First(child => child.name == "ScreenText").GetComponent<TextMeshProUGUI>();


        actionButton.onClick.AddListener(InnerOnActionButtonClick);  
    }

    public void SetActionButtonText(string text)
    {
        actionButtonText.text = text;
    }

    public void SetScreenText(string text)
    {
        screenText.text = text;
    }

    void InnerOnActionButtonClick()
    {
        OnActionButtonClick?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
