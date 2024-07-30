
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    bool isTouch = false;
    Vector3 ballStartPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballStartPos = transform.position;
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

}
