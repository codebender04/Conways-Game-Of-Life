using UnityEngine;
using UnityEngine.EventSystems;

public class UIMapZoom : MonoBehaviour, IScrollHandler
{
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 5f;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y;
        if (scroll == 0) return;

        Vector3 scale = rect.localScale;
        scale += Vector3.one * scroll * zoomSpeed;
        scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
        scale.y = Mathf.Clamp(scale.y, minScale, maxScale);

        rect.localScale = scale;
    }
}
