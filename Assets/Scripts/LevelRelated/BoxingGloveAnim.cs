using System.Collections;
using UnityEngine;

public class BoxingGloveAnim : MonoBehaviour
{
    public Transform backPosition; // The position the glove moves to when charging
    public Transform punchPosition; // The position the glove moves to when punching
    public GameObject glove; // The glove object
    public GameObject spring; // The spring object

    public float chargeTime = 2f; // Time it takes to charge the punch
    public float punchTime = 0.5f; // Time it takes to execute the punch
    public float waitTime = 1f; // Time to wait before repeating the process

    private Vector3 originalSpringScale;
    Puncher puncher;

    private void Start()
    {
        puncher = GetComponentInChildren<Puncher>();

        originalSpringScale = spring.transform.localScale;
        StartCoroutine(AnimateGlove());
    }

    private IEnumerator AnimateGlove()
    {
        while (true)
        {
            // Move glove to back position and scale spring to look like it's charging
            puncher.Damagable = false;
            yield return MoveAndScale(glove.transform, backPosition.position, punchTime, chargeTime, true);

            // Wait before punching
            yield return new WaitForSeconds(waitTime);
            puncher.Damagable = true;
            // Move glove to punch position and scale spring back to original size
            yield return MoveAndScale(glove.transform, punchPosition.position, punchTime, chargeTime, false);
            puncher.Damagable = false;
            // Wait before repeating the process
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveAndScale(Transform objectToMove, Vector3 targetPosition, float moveTime, float scaleTime, bool isCharging)
    {
        Vector3 startPosition = objectToMove.position;
        Vector3 targetScale = isCharging ? new Vector3(originalSpringScale.x, originalSpringScale.y, 0.1f) : originalSpringScale;
        Vector3 startScale = spring.transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < scaleTime)
        {
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
            spring.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / scaleTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.position = targetPosition;
        spring.transform.localScale = targetScale;
    }
}
