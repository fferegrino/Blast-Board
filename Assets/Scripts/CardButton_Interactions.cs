using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public class CardButton_Interactions : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    private Animator targetAnimator;
    private Animator cardRevealAnimator;

    public Image numberImage;
    private bool isTargeted = false;

    public Sprite mark0;
    public Sprite mark1;
    public Sprite mark2;
    public Sprite mark3;

    public void OnPointerDown(PointerEventData eventData)
    {
        // isTargeted = !isTargeted;
        // // targetAnimator.SetBool("IsTargeted", isTargeted);
        cardRevealAnimator.SetBool("IsRevealed", true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var allChildren = GetComponentsInChildren<Transform>();

        var target = allChildren.First(child => child.name == "Target");
        targetAnimator = target.GetComponent<Animator>();

        var cardReveal = allChildren.First(child => child.name == "CardCover");
        cardRevealAnimator = cardReveal.GetComponent<Animator>();
        // image = allChildren.First(child => child.name == "Number").GetComponent<Image>();
    }

    public void SetValue(int value)
    {   
        if (numberImage == null)
        {
            Debug.LogError($"Image not found for {name}");
            return;
        }
        
        if (value == 0)
        {
            numberImage.sprite = mark0;
        }
        else if (value == 1)
        {
            numberImage.sprite = mark1;
        }
        else if (value == 2)
        {
            numberImage.sprite = mark2;
        }
        else if (value == 3)
        {
            numberImage.sprite = mark3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
