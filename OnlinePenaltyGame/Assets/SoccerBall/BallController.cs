
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance;
    private Rigidbody rb;
    bool isTouch = false;
    Vector3 initialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initialPosition = transform.position;
            Debug.Log("BallIniitalPos" + initialPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void KickBall(Vector3 direction, float kickForce)
    {
        rb.AddForce(direction * kickForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoccerGoal"))
        {
            GameManager.Instance.UpdateScore();
        }
    }

    public void ResetPosition()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>(); // Emin olun ki rb null deðil
        }
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


}
