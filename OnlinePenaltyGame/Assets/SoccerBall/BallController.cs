using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float kickForce = 500f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void KickBall(Vector3 direction)
    {
        rb.AddForce(direction * kickForce);
    }
}
