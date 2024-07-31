using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] BallController ballController;
    [SerializeField] GoalKeeperController goalKeeperController;

    [SerializeField] Animator animator;
    [SerializeField] AnimationClip penaltyKickAnim;
    [SerializeField] AnimationClip idle;
    [SerializeField] Transform ball;
    [SerializeField] Transform goal;

    private Vector3 finalPosition;
    private Quaternion finalRotation;
    private bool animationFinished = false;

    Vector3 targetPosition;

    GameManager gameManager;
    PhotonView photonView;

    Vector3 initialPosition;
    Quaternion initialRotation;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initialPosition = transform.position;
            Debug.Log("PlayerInitialPos" + initialPosition);
            initialRotation = transform.rotation;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            // Eðer bu obje yerel oyuncuya ait deðilse, ShootControl panelini kapat.
            GameObject shootControlPanel = GameObject.Find("ShootControl");
            if (shootControlPanel != null)
            {
                shootControlPanel.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (animationFinished)
        {
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }
    }

    public void OnShootButtonPressed()
    {
        gameManager.SetPlayer1Done();

        // Renk bilgisi al ve iþleme devam et
        string arrowColor = gameManager.GetSliderArrowColor();
        Debug.Log("Slider Arrow Color: " + arrowColor);
        gameManager.StopSliderArrowMovement(out Vector3 arrowPos);

        // targetImage hareketini durdur ve pozisyon bilgisini al
        targetPosition = gameManager.StopTargetMovement();
    }

    public void StartShooting()
    {
        photonView.RPC("PunRPC_Shoot", RpcTarget.All);
    }

    [PunRPC]
    public void PunRPC_Shoot() // button icinde
    {
        // Animasyonu oynat
        animator.Play(penaltyKickAnim.name);
        animationFinished = false;


        targetPosition = gameManager.GetSliderArrowColor() == "Red" ? gameManager.FailShootMovement() : targetPosition;
        targetPosition = gameManager.GetSliderArrowColor() == "Blue" ? gameManager.BlueColorOptions() : targetPosition;
    }

    [PunRPC]
    public void PunRPC_ShootBall(Vector3 targetPos, float kickForce)
    {
        Vector3 direction = (targetPos - ball.position).normalized;
        ballController.KickBall(direction, 2, kickForce); // 2 high i temsil ediyor daha iyi bir degerle daha iyi goruntu cikarabilirsin 

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

    public void ResetPosition()
    {
        transform.position = initialPosition; // Oyuncunun baþlangýç pozisyonunu burada belirtin
        transform.rotation = initialRotation; // Oyuncunun baþlangýç rotasyonunu burada belirtin
        animator.Play(idle.name); // Oyuncunun animasyonunu baþlangýç durumuna getirin
    }

}
