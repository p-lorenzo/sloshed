using TMPro;
using UnityEngine;

public class ScoreEntry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;

    public void Initialize(string position, string name, string time)
    {
        positionText.text = position;
        nameText.text = name;
        timeText.text = time;
    }
}
