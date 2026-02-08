using UnityEngine;
using System;
using UnityEngine.UI;
public class MemoPad : MonoBehaviour
{
    public Button mark0Button;
    public Button mark1Button;
    public Button mark2Button;
    public Button mark3Button;

    void Awake()
    {
        mark0Button.onClick.AddListener(() => OnClick(CellMarks.Mark0));
        mark1Button.onClick.AddListener(() => OnClick(CellMarks.Mark1));
        mark2Button.onClick.AddListener(() => OnClick(CellMarks.Mark2));
        mark3Button.onClick.AddListener(() => OnClick(CellMarks.Mark3));
    }

    public event Action<CellMarks> OnMemoPadClick;

    public void OnClick(CellMarks mark)
    {
        OnMemoPadClick?.Invoke(mark);
    }
}
