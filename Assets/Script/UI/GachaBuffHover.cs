using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GachaBuffHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI label;
    private Color normalColor = Color.white;
    private Color hoverColor = Color.red;

    private void Awake()
    {
        label.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        label.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        label.color = normalColor;
    }
}