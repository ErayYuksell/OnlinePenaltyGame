using TMPro;
using UnityEngine;
using Photon.Pun;

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
    PhotonView photonView;
    [SerializeField] GameObject shootControll;
    bool allDone = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        photonView = GetComponent<PhotonView>();

        // Null kontrolü
        if (gameManager == null)
        {
            Debug.LogError("GameManager instance not found!");
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView component not found!");
        }
    }

    void Update()
    {
        if (animationFinished)
        {
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }
        if (!allDone)
        {
            gameManager.UpdatePlayerInfo();
            if (gameManager.GetPlayer1Info() && gameManager.GetPlayer2Info())
            {
                allDone = true;
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        gameManager.SetPlayer1Info(true);
        if (allDone)
        {
            photonView.RPC("PunRPC_Shoot", RpcTarget.All);
        }
    }
    [PunRPC]
    public void PunRPC_Shoot() // button icinde
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
    }

    [PunRPC]
    public void PunRPC_ShootBall(Vector3 targetPos, float kickForce)
    {
        Vector3 direction = (targetPos - ball.position).normalized;
        ballController.KickBall(direction, kickForce);

        // Animasyon tamamlandýðýnda yapýlacak iþlemler
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;

        // Idle state'e geçmeden önce pozisyonu ve rotasyonu sabitle
        animator.SetBool("isIdle", true);
    }

    // Animasyon Event tarafýndan çaðrýlacak metod
    public void OnKick()
    {
        // Topa vurma iþlemi artýk RPC ile senkronize edildiði için bu metod boþ býrakýlabilir.
        // Topa vurma iþlemini tüm oyunculara senkronize et
        photonView.RPC("PunRPC_ShootBall", RpcTarget.All, targetPosition, gameManager.BallMovementForceByColor());
    }

    // Animasyonun sonunda çaðrýlacak metod
    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;
        animator.SetBool("isIdle", true);
    }
}
