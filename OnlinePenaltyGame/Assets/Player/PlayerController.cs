using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] BallController ballController;
    [SerializeField] GoalKeeperController goalKeeperController;

    [SerializeField] Animator animator;
    [SerializeField] AnimationClip penaltyKickAnim;
    [SerializeField] Transform ball;
    [SerializeField] Transform goal;

    private Vector3 finalPosition;
    private Quaternion finalRotation;
    private bool animationFinished = false;

    Vector3 targetPosition;

    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    void Update()
    {
        if (animationFinished)
        {
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }
    }

    public void Shoot() // button icinde 
    {
        // Animasyonu oynat
        animator.Play(penaltyKickAnim.name);
        animationFinished = false;

        // Renk bilgisi al ve i�leme devam et
        string arrowColor = gameManager.GetSliderArrowColor();
        Debug.Log("Slider Arrow Color: " + arrowColor);
        gameManager.StopSliderArrowMovement(out Vector3 arrowPos);

        // targetImage hareketini durdur ve pozisyon bilgisini al

        targetPosition = gameManager.StopTargetMovement();
        targetPosition = gameManager.GetSliderArrowColor() == "Red" ? gameManager.FailShootMovement() : targetPosition;
        targetPosition = gameManager.GetSliderArrowColor() == "Blue" ? gameManager.BlueColorOptions() : targetPosition;

        // Animasyonun ortas�nda topa vurulmas�n� sa�lamak i�in animasyon event ekleyin
    }



    // Animasyon Event taraf�ndan �a�r�lacak metod
    public void OnKick()
    {
        Vector3 direction = (targetPosition - ball.position).normalized;
        float kickForce = gameManager.BallMovementForceByColor();

        ballController.KickBall(direction, kickForce);
    }


    // Animasyonun sonunda �a�r�lacak metod
    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;

        // Idle state'e ge�meden �nce pozisyonu ve rotasyonu sabitle
        animator.SetBool("isIdle", true);
    }
}
