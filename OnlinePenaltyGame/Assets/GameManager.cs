using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Points")]
    [SerializeField] List<GameObject> shootPoints = new List<GameObject>();
    [SerializeField] List<GameObject> failShootPoints = new List<GameObject>();
    //[SerializeField] RectTransform targetImage;
    public Transform targetObj;
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
    public enum ShootColors { red, green, blue }
    public ShootColors shootColors;

    float kickForce;
    [Header("Photon")]
    [SerializeField] GameObject shootControllPanel;
    [SerializeField] GameObject goalkeeperAreaPanel;
    PhotonView photonView;
    bool isPlayer1Done = false;
    bool isPlayer2Done = false;
    bool isPlayer1Turn = true;
    bool ballInside = false;
    [SerializeField] TextMeshProUGUI player1ScoreText;
    [SerializeField] TextMeshProUGUI player2ScoreText;
    int player1Score = 0;
    int player2Score = 0;
    [SerializeField] TextMeshProUGUI countdownText; // Yeni de�i�ken eklendi
    int countdownTimer = 10;
    bool isCountdownRunning = false; // Saya� durumunu takip eden de�i�ken
    bool isGameDone = true;
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
        photonView = GetComponent<PhotonView>();
        ChooseRandomPoint();
        MovementSliderArrow();
        MultiplayerController();
        StartCountdown();
    }

    #region TargetMovement

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
                    ChooseRandomPoint(point1, point2); // Hareket tamamland���nda yeni iki nokta se�ilir ve hareket tekrar ba�lar
                });
    }

    public Vector3 StopTargetMovement()
    {
        targetTween?.Kill(); // targetImage hareketini durdur
        targetObj.gameObject.SetActive(false);
        return targetObj.position; // O anki pozisyon bilgisini al
    }

    // Slider Movement and Color options
    public Vector3 FailShootMovement()
    {
        return failShootPoints[Random.Range(0, failShootPoints.Count)].gameObject.transform.position;
    }

    public Vector3 BlueColorOptions()
    {
        return targetObj.transform.position + Random.insideUnitSphere * 2f;
    }

    #endregion

    #region SliderMovementAndColor

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

    public float BallMovementForceByColor()
    {
        switch (GetSliderArrowColor())
        {
            default: return 500f;
            case "Red":
                kickForce = 500f;
                break;
            case "Blue":
                kickForce = 750f;
                break;
            case "Green":
                kickForce = 1000f;
                break;
        }
        return kickForce;
    }

    private bool IsWithinBounds(Vector3 arrowPos, RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners); // bu fonksiyon ile rect objesinin 4 kenarininin tam olarak konumunu alabiliyorsun 
        return arrowPos.x >= corners[0].x && arrowPos.x <= corners[2].x;
    }

    #endregion

    #region Multiplayer
    public void MultiplayerController()
    {
        if (!PhotonNetwork.IsMasterClient && isGameDone)
        {
            isGameDone = false;
            shootControllPanel.SetActive(false);
            targetObj.gameObject.SetActive(false);
            goalkeeperAreaPanel.SetActive(true);
        }
    }

    IEnumerator UpdatePlayerTurn()
    {
        if (isPlayer1Turn)
        {
            yield return new WaitForSeconds(1.5f);
            PhotonNetwork.LoadLevel("Game");
            shootControllPanel.SetActive(true);
            targetObj.gameObject.SetActive(true);
            goalkeeperAreaPanel.SetActive(false);
        }
        else if (!isPlayer1Turn)
        {
            yield return new WaitForSeconds(1.5f);
            PhotonNetwork.LoadLevel("Game");
            shootControllPanel.SetActive(false);
            targetObj.gameObject.SetActive(false);
            goalkeeperAreaPanel.SetActive(true);
        }
    }

    public void SetPlayer1Done()
    {
        photonView.RPC("PunRPC_SetPlayer1Done", RpcTarget.All);
    }

    public void SetPlayer2Done()
    {
        photonView.RPC("PunRPC_SetPlayer2Done", RpcTarget.All);
    }

    [PunRPC]
    void PunRPC_SetPlayer1Done()
    {
        isPlayer1Done = true;
        CheckAllPlayersReady();
    }

    [PunRPC]
    void PunRPC_SetPlayer2Done()
    {
        isPlayer2Done = true;
        CheckAllPlayersReady();
    }

    void CheckAllPlayersReady()
    {
        if (isPlayer1Done && isPlayer2Done)
        {
            photonView.RPC("PunRPC_StartShooting", RpcTarget.All);
        }
    }

    [PunRPC]
    void PunRPC_StartShooting()
    {
        // Her iki oyuncu da haz�rsa, shoot ve kaleci animasyonlar� ayn� anda ba�lar.
        PlayerController.Instance.StartShooting();
        GoalKeeperController.Instance.StartSaving();
    }

    #region ScoreSection

    public void UpdateScore()
    {
        if (isPlayer1Turn)
        {
            player1Score++;
        }
        else
        {
            player2Score++;
        }
        UpdateScoreText();
        isPlayer1Turn = !isPlayer1Turn;
        ballInside = true;
        StartCountdown(); // Yeni saya� ba�lat�ld�
        StartCoroutine(UpdatePlayerTurn());
    }

    public void BallNotInside()
    {
        if (!ballInside)
        {
            isPlayer1Turn = !isPlayer1Turn;
            StopCountdown(); // Saya� durduruldu
            StartCountdown(); // Yeni saya� ba�lat�ld�
            StartCoroutine(UpdatePlayerTurn());
        }
    }

    void UpdateScoreText()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
    }



    #endregion

    #region CountdownTimer

    [PunRPC]
    IEnumerator PunRPC_CountdownTimer()
    {
        isCountdownRunning = true; // Saya� ba�l�yor
        countdownTimer = 10; // Saya� s�resini ba�a al
        while (countdownTimer > 0)
        {
            countdownText.text = countdownTimer.ToString(); // Geri say�m� ekranda g�ster
            yield return new WaitForSeconds(1);
            countdownTimer--;
        }
        isCountdownRunning = false; // Saya� bitti
        countdownText.text = ""; // Saya� ekran�n� temizle
        // Ek i�lemler burada yap�labilir
    }

    public void StartCountdown()
    {
        photonView.RPC("PunRPC_CountdownTimer", RpcTarget.All);
    }

    public void StopCountdown()
    {
        if (isCountdownRunning)
        {
            StopCoroutine("PunRPC_CountdownTimer");
            photonView.RPC("PunRPC_StopCountdown", RpcTarget.All);
        }
    }

    [PunRPC]
    void PunRPC_StopCountdown()
    {
        isCountdownRunning = false;
        countdownText.text = "";
    }

    #endregion

    #endregion
}
