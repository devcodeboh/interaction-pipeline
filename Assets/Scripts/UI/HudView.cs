using UnityEngine;
using UnityEngine.UI;

public sealed class HudView : MonoBehaviour
{
    [SerializeField] private Text matchesText;
    [SerializeField] private Text turnsText;

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
