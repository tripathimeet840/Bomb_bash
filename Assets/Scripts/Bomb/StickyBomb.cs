using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonFX;

public class StickyBomb : MonoBehaviour
{
    public Rigidbody ball;
    public Transform target;

    public float h = 1;
    public float gravity = -40;

    public bool debugPath;
    public float lifeTime = 10f; // Changed to 10 seconds
    protected ObjectPooler objectPooler;
    float BombThrowerIndex;

    [Header("Damage")]
    public float yForce = 1;
    public float PlayerexplosionRadius = 3.5f;
    public float EnemyexplosionRadius = 3f;

    public float explosionForce = 20;
    protected Coroutine blastCorutine;
    public static StickyBomb lastSpawnedBomb;

    protected virtual void Start()
    {
        objectPooler = ObjectPooler.instance;
        lastSpawnedBomb = this; // register this bomb when spawned
    }

    public void SetData(Transform _target, int _index)
    {
        BombThrowerIndex = _index;
        target = _target;
        Launch();
    }

    private void OnDisable()
    {
        ball.useGravity = false;
        ball.linearVelocity = Vector3.zero;
        if (blastCorutine != null)
        {
            StopCoroutine(blastCorutine);
            blastCorutine = null;
        }
    }

    private void Launch()
    {
        FindObjectOfType<BombCollector>().EnableBlastButton();

        Physics.gravity = Vector3.up * gravity;
        ball.useGravity = true;
        Vector3 calculatedVelocity = CalculateLaunchData().initialVelocity;

        if (float.IsNaN(calculatedVelocity.x) || float.IsNaN(calculatedVelocity.y) || float.IsNaN(calculatedVelocity.z))
            calculatedVelocity = Vector3.zero;

        ball.linearVelocity = calculatedVelocity;

        // Always explode after "lifeTime" (10s) if not manually detonated
        blastCorutine = StartCoroutine(BlastAfterTime(lifeTime));
    }

    public static void DetonateLastBomb()
    {
        if (lastSpawnedBomb != null)
            lastSpawnedBomb.Detonate();
    }


    // Called from UI Button
    public void Detonate()
    {
        if (blastCorutine != null)
        {
            StopCoroutine(blastCorutine);
            blastCorutine = null;
        }

        Explode();
        FindObjectOfType<BombCollector>().DisableBlastButton();

    }

    // Shared explosion logic
    private void Explode()
    {
        GameObject bombvfx = objectPooler.GetObject("TimeBombVfx");
        bombvfx.transform.position = transform.position;
        bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = (BombThrowerIndex == 1);

        bombvfx.SetActive(true);
        gameObject.SetActive(false);

        if (BombThrowerIndex == 1)
            SoundManager.soundManager.PlayBombSound();
        else
            SoundManager.soundManager.PlayEnemyBombSound();

        CheckPlayersInContact();
    }

    protected IEnumerator BlastAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);
        Explode();
    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.position.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

    //protected IEnumerator BlastAfterTime(float _time)
    //{
    //    yield return new WaitForSeconds(_time);
    //    GameObject bombvfx = objectPooler.GetObject("TimeBombVfx");
    //    bombvfx.transform.position = transform.position;
    //    if (BombThrowerIndex == 1)
    //        bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = true;
    //    else
    //        bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = false;

    //    bombvfx.SetActive(true);
    //    gameObject.SetActive(false);
    //    if (BombThrowerIndex == 1)
    //        SoundManager.soundManager.PlayBombSound();
    //    else
    //        SoundManager.soundManager.PlayEnemyBombSound();

    //    CheckPlayersInContact();
    //}

    public void CheckPlayersInContact()
    {
        float explosiveRadius = BombThrowerIndex == 1 ? PlayerexplosionRadius : EnemyexplosionRadius;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            PlayerMovement playerMovement = hitCollider.GetComponentInParent<PlayerMovement>();
            if (playerMovement != null && playerMovement.PlayerIndex != BombThrowerIndex && !playerMovement.gettingDamage)
            {
                playerMovement.KnockBack();
                playerMovement.GetComponent<Health>().TakeDamage(playerMovement.EnemyPower);
                Rigidbody[] limbs = playerMovement.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody limb in limbs)
                {
                    Vector3 direction = (limb.transform.position - transform.position).normalized;
                    direction.y = yForce;
                    direction.Normalize();
                    limb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
            }

            AIMovement enemyMovement = hitCollider.GetComponentInParent<AIMovement>();
            if (enemyMovement != null && !enemyMovement.gettingDamage && enemyMovement.PlayerIndex != BombThrowerIndex)
            {
                enemyMovement.KnockBack();
                Rigidbody[] limbs = enemyMovement.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody limb in limbs)
                {
                    Vector3 direction = (limb.transform.position - transform.position).normalized;
                    direction.y = yForce;
                    direction.Normalize();
                    limb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
            }

            if (hitCollider.CompareTag("Impact"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                    ApplyExplosionForce(rb);

                if (hitCollider.transform.GetComponent<ObjectBehaviour>() != null)
                    hitCollider.transform.GetComponent<ObjectBehaviour>().DoAction();
            }

            if (hitCollider.CompareTag("TrainingEnemy"))
            {
                TrainingAIEnemy trainingAIEnemy = hitCollider.GetComponent<TrainingAIEnemy>();
                trainingAIEnemy.KnockBack();
                Rigidbody[] limbs = trainingAIEnemy.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody limb in limbs)
                {
                    Vector3 direction = (limb.transform.position - transform.position).normalized;
                    direction.y = yForce;
                    direction.Normalize();
                    limb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyexplosionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PlayerexplosionRadius);
    }

    // Removed all instant explosion triggers from OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        // Do nothing - only explode after timer
    }

    private void ApplyExplosionForce(Rigidbody rb)
    {
        Vector3 direction = (rb.transform.position - transform.position).normalized;
        direction.y = yForce;
        direction.Normalize();
        rb.AddForce(direction * explosionForce, ForceMode.Impulse);
    }
}
