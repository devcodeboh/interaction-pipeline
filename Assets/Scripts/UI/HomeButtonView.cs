using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class HomeButtonView : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action Clicked;

    private void OnEnable()
    {
        if (button != null) button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if (button != null) button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick() => Clicked?.Invoke();
}
