using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image backImage;
    [SerializeField] private Image faceImage;
    [SerializeField] private RectTransform flipRoot;
    [SerializeField] private float flipDuration = 0.25f;

    public event Action<int> Clicked;

    public int Index { get; private set; }
    public bool IsAnimating => isAnimating;
    public bool IsFaceUp => isFaceUp;

    private Coroutine flipRoutine;
    private bool isAnimating;
    private bool isFaceUp;

    public void Bind(int index)
    {
        Index = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(Index);
    }

    public void SetInstant(bool faceUp)
    {
        isFaceUp = faceUp;
        SetFaceVisibility(faceUp);
        SetScaleX(1f);
    }

    public void PlayFlip(bool faceUp)
    {
        if (!isAnimating && faceUp == isFaceUp)
            return;

        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        isAnimating = false;
        flipRoutine = StartCoroutine(FlipRoutine(faceUp));
    }

    private void Awake()
    {
        if (flipRoot == null)
            flipRoot = transform as RectTransform;

        SetInstant(false);
    }

    private IEnumerator FlipRoutine(bool faceUp)
    {
        isAnimating = true;
        float halfDuration = Mathf.Max(0.01f, flipDuration * 0.5f);

        float t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            SetScaleX(Mathf.Lerp(1f, 0f, t / halfDuration));
            yield return null;
        }

        SetFaceVisibility(faceUp);
        isFaceUp = faceUp;

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            SetScaleX(Mathf.Lerp(0f, 1f, t / halfDuration));
            yield return null;
        }

        SetScaleX(1f);
        isAnimating = false;
    }

    private void SetFaceVisibility(bool faceUp)
    {
        if (backImage != null)
            backImage.enabled = !faceUp;

        if (faceImage != null)
            faceImage.enabled = faceUp;
    }

    private void SetScaleX(float x)
    {
        if (flipRoot == null)
            return;

        Vector3 scale = flipRoot.localScale;
        scale.x = x;
        flipRoot.localScale = scale;
    }
}
