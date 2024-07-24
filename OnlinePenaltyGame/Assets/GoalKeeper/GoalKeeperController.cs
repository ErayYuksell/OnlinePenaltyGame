using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip bodyBlockAnim;
    [SerializeField] Transform yellowAreaTransform; // Sarý alaný temsil eden transform

    private Vector3 finalPosition;
    private Quaternion finalRotation;
    private bool animationFinished = false;

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

    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;
    }

    public void RotateYellowArea(float rotationFactor)
    {
        float rotationAngle = Mathf.Lerp(-45f, 45f, rotationFactor); // -45 ile 45 derece arasýnda dönüþ
        yellowAreaTransform.localEulerAngles = new Vector3(0, 0, rotationAngle);
    }
}
