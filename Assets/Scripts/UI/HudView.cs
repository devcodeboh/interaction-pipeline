using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI turnsText;

    public void SetMatches(int value)
    {
        if (matchesText == null)
            return;

        matchesText.text = $"Matches: {value}";
    }

    public void SetTurns(int value)
    {
        if (turnsText == null)
            return;

        turnsText.text = $"Turns: {value}";
    }
}
