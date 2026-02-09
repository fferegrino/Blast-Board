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
    public int Value => value;
    private int value;

    [Header("Mark icons (optional)")]
    [Tooltip("Icons shown when the user marks the cell. Enable/disable based on CellMarks.")]
    public GameObject markIcon0;
    public GameObject markIcon1;
    public GameObject markIcon2;
    public GameObject markIcon3;

    private int _row;
    private int _col;
    public int Row => _row;
    public int Column => _col;

    [Header("Sprites")]
    [Tooltip("Sprites for cell values 0 (bomb), 1, 2, 3")]
    public Sprite valueSprite0;
    public Sprite valueSprite1;
    public Sprite valueSprite2;
    public Sprite valueSprite3;

    

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var allChildren = GetComponentsInChildren<Transform>();

        var target = allChildren.First(child => child.name == "Target");
        targetAnimator = target.GetComponent<Animator>();

        var cardReveal = allChildren.First(child => child.name == "CardCover");
        cardRevealAnimator = cardReveal.GetComponent<Animator>();
    }

    /// <summary>
    /// Sets the card's visual state. Resets other states so only this one is active (e.g. clearing Targeted when Revealed).
    /// </summary>
    public void SetCellState(CellState cellState)
    {
        if (targetAnimator != null)
            targetAnimator.SetBool("IsTargeted", cellState == CellState.Targeted);

        if (cardRevealAnimator != null)
        {
            if (cellState == CellState.Revealed)
            {
                cardRevealAnimator.SetBool("IsExploded", value == 0);
                cardRevealAnimator.SetBool("IsRevealed", value != 0);
            }
            else
            {
                cardRevealAnimator.SetBool("IsExploded", false);
                cardRevealAnimator.SetBool("IsRevealed", false);
            }
        }
    }

    public void SetValue(int value)
    {
        this.value = value;
        if (numberImage == null) return;
        var sprite = value switch
        {
            0 => valueSprite0,
            1 => valueSprite1,
            2 => valueSprite2,
            3 => valueSprite3,
            _ => null
        };
        if (sprite != null)
            numberImage.sprite = sprite;
    }

    public void SetPosition(int row, int col)
    {
        _row = row;
        _col = col;
    }

    /// <summary>
    /// Shows or hides each mark icon. BoardController can pass gameState.HasCellMark(row, col, CellMarks.Mark0), etc.
    /// </summary>
    public void SetMark(bool showMark0, bool showMark1, bool showMark2, bool showMark3)
    {
        if (markIcon0 != null) markIcon0.SetActive(showMark0);
        if (markIcon1 != null) markIcon1.SetActive(showMark1);
        if (markIcon2 != null) markIcon2.SetActive(showMark2);
        if (markIcon3 != null) markIcon3.SetActive(showMark3);
    }
}
