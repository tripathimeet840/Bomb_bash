using System.Collections;
using UnityEngine;
using CartoonFX;

public class MiniBomb : MonoBehaviour
{
    public float lifeTime = 2f;        // explode after this many seconds
    public float explosionRadius = 2f; // smaller radius than big bomb
    public float explosionForce = 10f; // weaker force
    public float yForce = 1f;

    private ObjectPooler objectPooler;
    private Coroutine explodeRoutine;

    private void OnEnable()
    {
        objectPooler = ObjectPooler.instance;
        explodeRoutine = StartCoroutine(ExplodeAfterTime(lifeTime));
    }

    private void OnDisable()
    {
        if (explodeRoutine != null)
        {
            StopCoroutine(explodeRoutine);
            explodeRoutine = null;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator ExplodeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // spawn VFX
        GameObject vfx = objectPooler.GetObject("MiniBombVfx");
        vfx.transform.position = transform.position;
        vfx.SetActive(true);

        // damage + knockback
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Vector3 dir = (rb.transform.position - transform.position).normalized;
                dir.y = yForce;
                dir.Normalize();
                rb.AddForce(dir * explosionForce, ForceMode.Impulse);
            }
        }

        // sound
        SoundManager.soundManager.PlayBombSound();

        // disable after explosion
        gameObject.SetActive(false);
    }
}
