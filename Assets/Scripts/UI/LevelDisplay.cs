using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    public TextMeshProUGUI ScoreboardText;
    public TextMeshProUGUI ScoreboardValue;


    public void SetLevel(int level)
    {
        ScoreboardValue.text = string.Format("{0:D3}", level);
    }

}
