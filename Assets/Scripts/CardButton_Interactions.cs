using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CardButton_Interactions : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    private Animator targetAnimator;
    private Animator cardRevealAnimator;
    private bool isTargeted = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        // isTargeted = !isTargeted;
        // // targetAnimator.SetBool("IsTargeted", isTargeted);
        // // cardRevealAnimator.SetBool("IsRevealed", true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var allChildren = GetComponentsInChildren<Transform>();

        var target = allChildren.First(child => child.name == "Target");
        targetAnimator = target.GetComponent<Animator>();

        var cardReveal = allChildren.First(child => child.name == "CardCover");
        cardRevealAnimator = cardReveal.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
