using UnityEditor.Timeline.Actions;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void KickBall(Vector3 direction, float kickForce)
    {
        rb.AddForce(direction * kickForce);
    }

}
