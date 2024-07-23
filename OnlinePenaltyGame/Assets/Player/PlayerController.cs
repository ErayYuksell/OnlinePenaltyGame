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
        animator.Play(penaltyKickAnim.name);
        animationFinished = false;

        // Animasyonun ortas�nda topa vurulmas�n� sa�lamak i�in animasyon event ekleyin
    }

    // Animasyon Event taraf�ndan �a�r�lacak metod
    public void OnKick()
    {
        Vector3 direction = (goal.position - ball.position).normalized;
        ballController.KickBall(direction);
        goalKeeperController.PlayBodyBlock();
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
