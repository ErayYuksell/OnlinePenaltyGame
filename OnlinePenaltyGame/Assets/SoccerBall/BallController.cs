using UnityEditor.Timeline.Actions;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float kickForce = 500f;
    private Rigidbody rb;

    Transform targetObj;

    bool shouldDrawGizmo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetObj = GameManager.Instance.targetObj;
    }

    public void KickBall(Vector3 direction)
    {
        float force = BallMovementByColor();
        Debug.Log(force.ToString());
        rb.AddForce(direction * force);
    }

    public float BallMovementByColor()
    {
        switch (GameManager.Instance.GetSliderArrowColor())
        {
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

    public Vector3 BlueColorOptions()
    {
        shouldDrawGizmo = true;
        Vector3 randomPoint;
        return randomPoint = targetObj.transform.position + Random.insideUnitSphere * 1f;
    }


    private void OnDrawGizmos()
    {
        if (shouldDrawGizmo && targetObj != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetObj.position, 1);
        }
    }
}
