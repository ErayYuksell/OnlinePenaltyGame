using Photon.Pun;
using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    public static GoalKeeperController Instance;

    [SerializeField] Animator animator;
    [SerializeField] AnimationClip bodyBlockAnim;
    [SerializeField] AnimationClip bodyBlockRightAnim;
    [SerializeField] AnimationClip divingSave;
    [SerializeField] AnimationClip divingRightSave;
    [SerializeField] AnimationClip catchAnim;
    [SerializeField] Transform yellowAreaParentTransform; // Sarý alaný temsil eden transform

    private Vector3 finalPosition;
    private Quaternion finalRotation;
    private bool animationFinished = false;

    PhotonView photonView;

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
    }

    private void Update()
    {
        if (animationFinished)
        {
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }
    }

    public void PlayBodyBlock()
    {
        animator.Play(bodyBlockAnim.name);
    }
    public void PlayBodyRightBlock()
    {
        animator.Play(bodyBlockRightAnim.name);
    }
    public void PlayDivingSave()
    {
        animator.Play(divingSave.name);
    }
    public void PlayDivingRightSave()
    {
        animator.Play(divingRightSave.name);
    }
    public void PlayCatch()
    {
        animator.Play(catchAnim.name);
    }

    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;
    }

    public void RotateYellowArea(float rotationFactor)
    {
        float rotationAngle = Mathf.Lerp(-60f, 60f, -rotationFactor * 0.5f + 0.5f);
        yellowAreaParentTransform.localEulerAngles = new Vector3(0, 0, rotationAngle);
        photonView.RPC("PunRPC_UpdateYellowAreaRotation", RpcTarget.All, yellowAreaParentTransform.localEulerAngles.z);
    }

    [PunRPC]
    public void PunRPC_UpdateYellowAreaRotation(float rotationZ)
    {
        yellowAreaParentTransform.localEulerAngles = new Vector3(0, 0, rotationZ);
    }


    public void StartSaving()
    {
        float yellowAreaRotationZ = yellowAreaParentTransform.localEulerAngles.z;
        if (yellowAreaRotationZ > 180) yellowAreaRotationZ -= 360;
        Debug.Log(yellowAreaRotationZ);

        // Sarý alanýn dönüþ açýsýna göre animasyonlarý belirle
        if (IsInRange(yellowAreaRotationZ, -20, 20))
        {
            PlayCatch();
        }
        else if (IsInRange(yellowAreaRotationZ, -40, -20) || IsInRange(yellowAreaRotationZ, 20, 40))
        {
            if (IsInRange(yellowAreaRotationZ, -40, -20))
            {
                PlayDivingRightSave();
            }
            else
            {
                PlayDivingSave();
            }
        }
        else if (IsInRange(yellowAreaRotationZ, -61, -40) || IsInRange(yellowAreaRotationZ, 40, 61))
        {
            if (IsInRange(yellowAreaRotationZ, -61, -40))
            {
                PlayBodyRightBlock();
            }
            else
            {
                PlayBodyBlock();
            }
        }
        else
        {
            PlayCatch(); // Varsayýlan animasyon
        }

        // Belirtilen aralýkta olup olmadýðýný kontrol eden yardýmcý metot
        bool IsInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}
