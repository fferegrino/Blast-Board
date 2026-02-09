using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialPagesManager : MonoBehaviour
{

    private GameObject[] tutorialPages;

    private int currentPage = 0;

    public Button backButton;
    public Button nextButton;

    private TextMeshProUGUI backButtonText; 
    private TextMeshProUGUI nextButtonText; 

    void Awake()
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

        backButtonText = backButton.GetComponent<TextMeshProUGUI>();
        nextButtonText = nextButton.GetComponent<TextMeshProUGUI>();
        ShowPage(currentPage);

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

    public void ShowPage(int pageNumber)
    {
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].SetActive(i == pageNumber);
        }

        if (pageNumber == 0)
        {
            backButtonText.text = "Exit";
        }
        else
        {
            backButtonText.text = "Back";
        }
        if (pageNumber == tutorialPages.Length - 1)
        {
            nextButtonText.text = "Exit";
        }
        else
        {
            nextButtonText.text = "Next";
        }
    }
}
