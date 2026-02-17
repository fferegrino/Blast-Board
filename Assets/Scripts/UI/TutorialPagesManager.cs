using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class TutorialPagesManager : MonoBehaviour
{

    private GameObject[] tutorialPages;

    private int currentPage = 0;

    [Header("Buttons")]
    public Button backButton;
    public Button nextButton;

    [Header("Button Texts")]
    public TextMeshProUGUI backButtonText;
    public TextMeshProUGUI nextButtonText;

    [Header("Texts")]
    public LocalizedString nextText;
    public LocalizedString backText;
    public LocalizedString exitText;

    private string innerNextText;
    private string innerBackText;
    private string innerExitText;


    void Start()
    {
        var allChildren = GetComponentsInChildren<Transform>(true);
        var pages = allChildren.Where(
            child => child.name.StartsWith("Page") && child.parent == this.transform).Select(child => child.gameObject);
        // Sort pages by number
        pages = pages.OrderBy(page => int.Parse(page.name.Substring(4)));
        tutorialPages = pages.ToArray();

        currentPage = 0;

        backButton.onClick.AddListener(OnBackButtonClick);
        nextButton.onClick.AddListener(OnNextButtonClick);

        ShowPage(currentPage);

        nextText.StringChanged += OnNextTextChanged;
        backText.StringChanged += OnBackTextChanged;
        exitText.StringChanged += OnExitTextChanged;

    }

    void OnNextTextChanged(string value)
    {
        innerNextText = value;
        SetNextButtonText();
    }

    void OnBackTextChanged(string value)
    {
        innerBackText = value;
        SetBackButtonText();
    }

    void OnExitTextChanged(string value)
    {
        innerExitText = value;
        SetBackButtonText();
        SetNextButtonText();
    }

    void OnBackButtonClick()
    {
        if (currentPage == 0)
        {
            SceneManager.LoadScene("GameScene");
            return;
        }
        currentPage--;
        ShowPage(currentPage);
    }

    void OnNextButtonClick()
    {
        if (currentPage == tutorialPages.Length - 1)
        {
            SceneManager.LoadScene("GameScene");
            return;
        }
        currentPage++;
        ShowPage(currentPage);
    }

    void SetBackButtonText()
    {
        if (backButtonText != null) {
            if (currentPage == 0) {
                backButtonText.text = innerExitText;
            }
            else {
                backButtonText.text = innerBackText;
            }
        }
    }
    void SetNextButtonText()
    {
        if (nextButtonText != null) {
            if (currentPage == tutorialPages.Length - 1) {
                nextButtonText.text = innerExitText;
            }
            else {
                nextButtonText.text = innerNextText;
            }
        }
    }

    public void ShowPage(int pageNumber)
    {
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].SetActive(i == pageNumber);
        }
        SetBackButtonText();
        SetNextButtonText();
    }
}
