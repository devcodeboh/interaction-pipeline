using UnityEngine;

public sealed class WinPopupView : MonoBehaviour
{
    [SerializeField] private NextButtonView nextButton;

    public NextButtonView NextButton => nextButton;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
