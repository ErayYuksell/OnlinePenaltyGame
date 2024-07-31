using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance;
    private Rigidbody rb;
    private Vector3 initialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initialPosition = transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void KickBall(Vector3 targetPosition, float height, float duration)
    {
        StartCoroutine(SmoothKickBall(targetPosition, height, duration));
    }

    private IEnumerator SmoothKickBall(Vector3 targetPosition, float height, float duration)
    {
        rb.isKinematic = true; // Kinematic durumu açýlýyor
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // Parabolik hareket için Lerp kullanýmý
            float t = elapsedTime / duration;
            float yOffset = height * Mathf.Sin(Mathf.PI * t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + new Vector3(0, yOffset, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        rb.isKinematic = false; // Kinematic durumu kapatýlýyor
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

        rb.isKinematic = false; // Kinematic durumu kapatýlýyor
        rb.velocity = Vector3.zero; // Topun hareketini durdurun
        rb.angularVelocity = Vector3.zero; // Topun dönme hareketini durdurun
        transform.position = initialPosition; // Topun baþlangýç pozisyonunu burada belirtin
        rb.isKinematic = true; // Kinematic durumu tekrar açýlýyor
    }

}
