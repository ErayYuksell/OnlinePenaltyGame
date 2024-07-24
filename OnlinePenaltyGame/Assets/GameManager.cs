using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Points")]
    [SerializeField] List<GameObject> shootPoints = new List<GameObject>();
    //[SerializeField] RectTransform targetImage;
    [SerializeField] Transform targetObj;
    private Tween targetTween;
    [Header("Slider")]
    [SerializeField] RectTransform sliderArrow;
    [SerializeField] float sliderStart;
    [SerializeField] float sliderFinish;
    private Tween sliderArrowTween;
    [Header("Slider Colors")]
    [SerializeField] RectTransform redImage;
    [SerializeField] RectTransform blueImage;
    [SerializeField] RectTransform greenImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChooseRandomPoint();
        MovementSliderArrow();
    }

    public void ChooseRandomPoint(GameObject oldPoint1 = null, GameObject oldPoint2 = null)
    {
        if (shootPoints.Count < 2)
        {
            Debug.LogError("Not enough shoot points.");
            return;
        }

        GameObject point1, point2;
        do
        {
            point1 = shootPoints[Random.Range(0, shootPoints.Count)];
            point2 = shootPoints[Random.Range(0, shootPoints.Count)];
        } while ((oldPoint1 != null && (point1 == oldPoint1 || point2 == oldPoint1)) ||
                 (oldPoint2 != null && (point1 == oldPoint2 || point2 == oldPoint2)) ||
                 (point1 == point2));

        MovementBetweenPoints(point1, point2);
    }

    

    public void MovementBetweenPoints(GameObject point1, GameObject point2)
    {

        targetTween?.Kill(); // Mevcut hareketi durdur

        Sequence sequence = DOTween.Sequence(); // dotween sirasi veya dizisi birden fazla dotween i birlikte kullanmak istiyorsan sirayla calisirlar 
        targetTween = sequence.Append(targetObj.DOMove(point1.transform.position, 1f).SetEase(Ease.Linear))
                .Append(targetObj.DOMove(point2.transform.position, 1f).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    ChooseRandomPoint(point1, point2); // Hareket tamamlandýðýnda yeni iki nokta seçilir ve hareket tekrar baþlar
                });
    }
    public Vector3 StopTargetMovement()
    {
        targetTween?.Kill(); // targetImage hareketini durdur
        return targetObj.position; // O anki pozisyon bilgisini al
    }
    // UI da bir target Image olusturup onun pointsler arasi hareket etmesini sagladim 
    //public void MovementBetweenPoints(GameObject point1, GameObject point2)
    //{
    //    Vector2 screenPos1 = Camera.main.WorldToScreenPoint(point1.transform.position); // world pos u olan normal objelerin posunu screen posa cevirip target Image uzerinden hareket ettiriyorum 
    //    Vector2 screenPos2 = Camera.main.WorldToScreenPoint(point2.transform.position);

    //    targetTween?.Kill(); // Mevcut hareketi durdur

    //    Sequence sequence = DOTween.Sequence(); // dotween sirasi veya dizisi birden fazla dotween i birlikte kullanmak istiyorsan sirayla calisirlar 
    //    targetTween = sequence.Append(targetImage.DOMove(screenPos1, 1f).SetEase(Ease.Linear))
    //            .Append(targetImage.DOMove(screenPos2, 1f).SetEase(Ease.Linear))
    //            .OnComplete(() =>
    //            {
    //                ChooseRandomPoint(point1, point2); // Hareket tamamlandýðýnda yeni iki nokta seçilir ve hareket tekrar baþlar
    //            });
    //}
    //public Vector3 StopTargetImageMovement()
    //{
    //    targetTween?.Kill(); // targetImage hareketini durdur
    //    return Camera.main.ScreenToWorldPoint(targetImage.position); // O anki pozisyon bilgisini al
    //}

    //Slider

    public void MovementSliderArrow()
    {
        Vector3 startPos = new Vector3(sliderStart, sliderArrow.localPosition.y, sliderArrow.localPosition.z);
        Vector3 finishPos = new Vector3(sliderFinish, sliderArrow.localPosition.y, sliderArrow.localPosition.z);
        sliderArrow.localPosition = startPos; // baska normal pos olarak almaya calistim alakasiz yerlerde git gel yapti o yuzden local aliyoruz
        sliderArrowTween = sliderArrow.DOLocalMove(finishPos, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    public void StopSliderArrowMovement(out Vector3 arrowPos)
    {
        sliderArrowTween?.Kill();
        arrowPos = sliderArrow.localPosition; // O anki pozisyon bilgisini al
    }

    // Renk belirleme metodu
    public string GetSliderArrowColor()
    {
        Vector3 arrowPos = sliderArrow.position;

        if (IsWithinBounds(arrowPos, redImage))
        {
            return "Red";
        }
        else if (IsWithinBounds(arrowPos, blueImage))
        {
            return "Blue";
        }
        else if (IsWithinBounds(arrowPos, greenImage))
        {
            return "Green";
        }

        return "Unknown";
    }

    private bool IsWithinBounds(Vector3 arrowPos, RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners); // bu fonksiyon ile rect objesinin 4 kenarininin tam olarak konumunu alabiliyorsun 
        return arrowPos.x >= corners[0].x && arrowPos.x <= corners[2].x;
    }
}
