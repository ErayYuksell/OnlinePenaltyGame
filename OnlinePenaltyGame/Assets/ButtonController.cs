using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] GameObject goalkeeperAreaPanel;
    [SerializeField] RectTransform buttonRectTransform; // Butonun RectTransform'u
    [SerializeField] RectTransform sliderRectTransform; // Slider'ýn RectTransform'u
    [SerializeField] GoalKeeperController goalKeeperController; // GoalKeeperController referansý
    [SerializeField] Transform yellowAreaParentTransform;

    private Vector2 initialButtonPosition;
    private Vector2 buttonStartPosition;

    bool isDrag = true;

    private void Start()
    {
        initialButtonPosition = buttonRectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            buttonStartPosition = buttonRectTransform.anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            Vector2 newPosition = buttonRectTransform.anchoredPosition + new Vector2(eventData.delta.x, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, -sliderRectTransform.rect.width / 2, sliderRectTransform.rect.width / 2);
            buttonRectTransform.anchoredPosition = newPosition;

            float rotationFactor = newPosition.x / (sliderRectTransform.rect.width / 2);
            goalKeeperController.RotateYellowArea(rotationFactor); // GoalKeeperController'daki RotateYellowArea fonksiyonunu çaðýr
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        goalkeeperAreaPanel.SetActive(false); // kaleci atlamadan paneli kapa 
        isDrag = false;
        buttonRectTransform.gameObject.GetComponent<Button>().interactable = false;

        float yellowAreaRotationZ = yellowAreaParentTransform.localEulerAngles.z;
        if (yellowAreaRotationZ > 180) yellowAreaRotationZ -= 360;
        Debug.Log(yellowAreaRotationZ);

        // Sarý alanýn dönüþ açýsýna göre animasyonlarý belirle
        if (IsInRange(yellowAreaRotationZ, -20, 20))
        {
            goalKeeperController.PlayCatch();
        }
        else if (IsInRange(yellowAreaRotationZ, -40, -20) || IsInRange(yellowAreaRotationZ, 20, 40))
        {
            if (IsInRange(yellowAreaRotationZ, -40, -20))
            {
                goalKeeperController.PlayDivingRightSave();
            }
            else
            {
                goalKeeperController.PlayDivingSave();
            }
        }
        else if (IsInRange(yellowAreaRotationZ, -61, -40) || IsInRange(yellowAreaRotationZ, 40, 61))
        {
            if (IsInRange(yellowAreaRotationZ, -61, -40))
            {
                goalKeeperController.PlayBodyRightBlock();
            }
            else
            {
                goalKeeperController.PlayBodyBlock();
            }
        }
        else
        {
            goalKeeperController.PlayCatch(); // Varsayýlan animasyon
        }
    }

    // Belirtilen aralýkta olup olmadýðýný kontrol eden yardýmcý metot
    private bool IsInRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

}
