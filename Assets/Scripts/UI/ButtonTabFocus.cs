using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTabFocus : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI bottomText;
    [SerializeField] private Button button;
    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            buttonImage.transform.DOPunchPosition(new Vector3(0f, 10f, 0), 0.2f);
        });
    }
    public void SetFocusButton(string text)
    {
        bottomText.text = text;

        RectTransform rect = buttonImage.rectTransform;

        rect.DOKill();

        rect.sizeDelta = new Vector2(140f, 60f);
        rect.DOSizeDelta(new Vector2(240f, 80f), 0.2f).SetEase(Ease.OutQuint);
    }
}
