using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonFX;

public class EnemyStickyBomb : MonoBehaviour
{
    public Rigidbody ball;
    public Transform target;

    public float h = 1;
    public float gravity = -40;

    public bool debugPath;
    public float lifeTime = 10f; // Always explode after 10s

    protected ObjectPooler objectPooler;
    float BombThrowerIndex;

    [Header("Damage")]
    public float yForce = 1;
    public float PlayerexplosionRadius = 3.5f;
    public float EnemyexplosionRadius = 3f;
    public float explosionForce = 20;

    protected Coroutine blastCorutine;

    protected virtual void Start()
    {
        objectPooler = ObjectPooler.instance;
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

    protected virtual void Launch()
    {
        Physics.gravity = Vector3.up * gravity;
        ball.useGravity = true;

        Vector3 calculatedVelocity = CalculateLaunchData().initialVelocity;
        if (float.IsNaN(calculatedVelocity.x) || float.IsNaN(calculatedVelocity.y) || float.IsNaN(calculatedVelocity.z))
            calculatedVelocity = Vector3.zero;

        ball.linearVelocity = calculatedVelocity;

        // Always explode after "lifeTime"
        blastCorutine = StartCoroutine(BlastAfterTime(lifeTime));
    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.position.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(
            target.position.x - transform.position.x,
            0,
            target.position.z - transform.position.z
        );

        float time = Mathf.Sqrt(-2 * h / gravity) +
                     Mathf.Sqrt(2 * (displacementY - h) / gravity);

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

    protected IEnumerator BlastAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);

        GameObject bombvfx = objectPooler.GetObject("TimeBombVfx");
        bombvfx.transform.position = transform.position;

        if (BombThrowerIndex == 1)
            bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = true;
        else
            bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = false;

        bombvfx.SetActive(true);
        gameObject.SetActive(false);

        if (BombThrowerIndex == 1)
            SoundManager.soundManager.PlayBombSound();
        else
            SoundManager.soundManager.PlayEnemyBombSound();

        CheckPlayersInContact();
    }

    public void CheckPlayersInContact()
    {
        float explosiveRadius = BombThrowerIndex == 1 ? PlayerexplosionRadius : EnemyexplosionRadius;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Player hit
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

            // Enemy hit
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

            // Impact objects
            if (hitCollider.CompareTag("Impact"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null) ApplyExplosionForce(rb);

                ObjectBehaviour obj = hitCollider.transform.GetComponent<ObjectBehaviour>();
                if (obj != null) obj.DoAction();
            }

            // Training dummy enemy
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

    // No instant explosion on collision, timer only
    private void OnCollisionEnter(Collision collision) { }

    private void ApplyExplosionForce(Rigidbody rb)
    {
        Vector3 direction = (rb.transform.position - transform.position).normalized;
        direction.y = yForce;
        direction.Normalize();
        rb.AddForce(direction * explosionForce, ForceMode.Impulse);
    }
}
