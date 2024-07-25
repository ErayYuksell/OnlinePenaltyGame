using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip bodyBlockAnim;
    [SerializeField] AnimationClip bodyBlockRightAnim;
    [SerializeField] AnimationClip divingSave;
    [SerializeField] AnimationClip divingRightSave;
    [SerializeField] AnimationClip catchAnim;
    [SerializeField] Transform yellowAreaParentTransform; // Sar� alan� temsil eden transform

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
        // rotationFactor'� tersine �evirerek sar� alan�n do�ru y�nde hareket etmesini sa�l�yoruz
        float rotationAngle = Mathf.Lerp(-60f, 60f, -rotationFactor * 0.5f + 0.5f);
        yellowAreaParentTransform.localEulerAngles = new Vector3(0, 0, rotationAngle);
    }
}
