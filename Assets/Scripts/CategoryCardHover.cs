using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CategoryCardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    public Sprite normalSprite;
    public Sprite hoverSprite;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null && hoverSprite != null)
            image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null && normalSprite != null)
            image.sprite = normalSprite;
    }
}
