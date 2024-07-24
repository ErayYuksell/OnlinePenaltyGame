using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform buttonRectTransform; // Butonun RectTransform'u
    [SerializeField] RectTransform sliderRectTransform; // Slider'�n RectTransform'u
    [SerializeField] GoalKeeperController goalKeeperController; // GoalKeeperController referans�

    private Vector2 initialButtonPosition;
    private Vector2 buttonStartPosition;

    private void Start()
    {
        initialButtonPosition = buttonRectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        buttonStartPosition = buttonRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = buttonRectTransform.anchoredPosition + new Vector2(eventData.delta.x, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, -sliderRectTransform.rect.width / 2, sliderRectTransform.rect.width / 2);
        buttonRectTransform.anchoredPosition = newPosition;

        float rotationFactor = newPosition.x / (sliderRectTransform.rect.width / 2);
        goalKeeperController.RotateYellowArea(rotationFactor); // GoalKeeperController'daki RotateYellowArea fonksiyonunu �a��r
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Drag i�lemi bitti�inde yap�lacak i�lemler
    }
}
