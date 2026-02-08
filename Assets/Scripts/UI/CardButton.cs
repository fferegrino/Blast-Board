using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using System;

public class CardButton : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    public event Action<CardButton> OnClick;
    private Animator targetAnimator;
    private Animator cardRevealAnimator;

    public Image numberImage;
    private bool isTargeted = false;

    private int value;

    private int _row;
    private int _col;
    public int Row => _row;
    public int Column => _col;

    public Sprite mark0;
    public Sprite mark1;
    public Sprite mark2;
    public Sprite mark3;

    

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
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

    public void SetCellState(CellState cellState)
    {
        switch (cellState)
        {
            case CellState.Hidden:
                cardRevealAnimator.SetBool("IsHidden", true);
                break;
            case CellState.Revealed:
                if (value == 0)
                {
                    cardRevealAnimator.SetBool("IsExploded", true);
                }
                else
                {
                    cardRevealAnimator.SetBool("IsRevealed", true);
                }
                break;
            case CellState.Targeted:
                targetAnimator.SetBool("IsTargeted", true);
                break;
        }
    }

    public void SetValue(int value)
    {   
        if (numberImage == null)
        {
            Debug.LogError($"Image not found for {name}");
            return;
        }
        this.value = value;
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

    public void SetPosition(int row, int col)
    {
        this._row = row;
        this._col = col;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
