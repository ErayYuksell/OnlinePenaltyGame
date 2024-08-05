using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] BallController ballController;
    [SerializeField] GoalKeeperController goalKeeperController;

    [SerializeField] Animator animator;
    [SerializeField] AnimationClip penaltyKickAnim;
    [SerializeField] AnimationClip idle;
    [SerializeField] AnimationClip offensiveIdle;
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
            // E�er bu obje yerel oyuncuya ait de�ilse, ShootControl panelini kapat.
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
        if (gameManager.GetPlayer1TurnInfo())
        {
            gameManager.SetPlayer1Done();
        }
        else if (gameManager.GetPlayer2TurnInfo())
        {
            gameManager.SetPlayer2Done();
        }

        // Renk bilgisi al ve i�leme devam et
        string arrowColor = gameManager.GetSliderArrowColor();
        Debug.Log("Slider Arrow Color: " + arrowColor + "_ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);
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
        Debug.Log("Penalty Animasyonu calisti" + "_ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);
        animationFinished = false;


        targetPosition = gameManager.GetSliderArrowColor() == "Red" ? gameManager.FailShootMovement() : targetPosition;
        targetPosition = gameManager.GetSliderArrowColor() == "Blue" ? gameManager.BlueColorOptions() : targetPosition;
    }

    [PunRPC]
    public void PunRPC_ShootBall(Vector3 targetPos, float kickForce)
    {
        Vector3 direction = (targetPos - ball.position).normalized;
        Vector3 finalForce = direction * kickForce; // Final kuvveti belirleniyor
        ballController.KickBall(targetPos, 2f, 1f, finalForce); // 2 high i temsil ediyor, 1 duration, daha iyi bir degerle daha iyi goruntu cikarabilirsin 
        Debug.Log("Top hareketi basladi" + "_ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);


        // Animasyon tamamland���nda yap�lacak i�lemler
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;

        // Idle state'e ge�meden �nce pozisyonu ve rotasyonu sabitle
        animator.Play(idle.name);
    }


    // Animasyon Event taraf�ndan �a�r�lacak metod
    public void OnKick()
    {
        // Topa vurma i�lemini t�m oyunculara senkronize et
        photonView.RPC("PunRPC_ShootBall", RpcTarget.All, targetPosition, gameManager.BallMovementForceByColor());
    }

    // Animasyonun sonunda �a�r�lacak metod
    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;
        animator.Play(idle.name);

        //StartCoroutine(BallNotInside());
    }

    IEnumerator BallNotInside()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.BallNotInside();
    }

    public void ResetPosition()
    {
        transform.position = initialPosition; // Başlangıç pozisyonuna sıfırla
        transform.rotation = initialRotation; // Başlangıç rotasyonuna sıfırla

        animator.Play(offensiveIdle.name);
        //Debug.Log("Kaleci pozisyonu ve rotasyonu sıfırlandı.");
    }

}
