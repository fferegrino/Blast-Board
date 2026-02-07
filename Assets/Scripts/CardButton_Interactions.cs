using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CardButton_Interactions : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    private Animator targetAnimator;
    private bool isTargeted = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        isTargeted = !isTargeted;
        targetAnimator.SetBool("IsTargeted", isTargeted);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var allChildren = GetComponentsInChildren<Transform>();
        var target = allChildren.First(child => child.name == "Target");
        targetAnimator = target.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
