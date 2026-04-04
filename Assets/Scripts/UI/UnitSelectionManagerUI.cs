using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaImage;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += InstanceOnOnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += InstanceOnOnSelectionAreaEnd;
        selectionAreaImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart -= InstanceOnOnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd -= InstanceOnOnSelectionAreaEnd;
    }

    private void Update()
    {
        if (selectionAreaImage.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void InstanceOnOnSelectionAreaEnd()
    {
        selectionAreaImage.gameObject.SetActive(false);
    }

    private void InstanceOnOnSelectionAreaStart()
    {
        selectionAreaImage.gameObject.SetActive(true);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
        float canvasScale = canvas.transform.localScale.x;

        selectionAreaImage.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y)/canvasScale;
        selectionAreaImage.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height)/canvasScale;

    }

}
