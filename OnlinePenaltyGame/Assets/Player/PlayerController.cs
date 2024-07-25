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

        // Renk bilgisi al ve iþleme devam et
        string arrowColor = gameManager.GetSliderArrowColor();
        Debug.Log("Slider Arrow Color: " + arrowColor);
        gameManager.StopSliderArrowMovement(out Vector3 arrowPos);

        // targetImage hareketini durdur ve pozisyon bilgisini al

        targetPosition = gameManager.StopTargetMovement();
        targetPosition = gameManager.GetSliderArrowColor() == "Red" ? gameManager.FailShootMovement() : targetPosition;
        targetPosition = gameManager.GetSliderArrowColor() == "Blue" ? gameManager.BlueColorOptions() : targetPosition;

        // Animasyonun ortasýnda topa vurulmasýný saðlamak için animasyon event ekleyin
    }



    // Animasyon Event tarafýndan çaðrýlacak metod
    public void OnKick()
    {
        Vector3 direction = (targetPosition - ball.position).normalized;
        float kickForce = gameManager.BallMovementForceByColor();

        ballController.KickBall(direction, kickForce);
    }


    // Animasyonun sonunda çaðrýlacak metod
    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;

        // Idle state'e geçmeden önce pozisyonu ve rotasyonu sabitle
        animator.SetBool("isIdle", true);
    }
}
