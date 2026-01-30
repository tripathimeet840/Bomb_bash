using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonFX;

public class FreezeBomb : MonoBehaviour
{
    public Rigidbody ball;
    public Transform target;

    public float h = 1;
    public float gravity = -40;

    public bool debugPath;
    public float lifeTime = 0.75f;
    protected ObjectPooler objectPooler;
    float BombThrowerIndex;

    [Header("Freeze Settings")]
    public float PlayerexplosionRadius = 3.5f;
    public float EnemyexplosionRadius = 3f;
    public float freezeDuration = 3f; // how long enemies/players stay frozen

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
        {
            calculatedVelocity = Vector3.zero;
        }

        ball.linearVelocity = calculatedVelocity;
        blastCorutine = StartCoroutine(BlastAfterTime(lifeTime));
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

    protected IEnumerator BlastAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);

        GameObject bombvfx = objectPooler.GetObject("FreezeBombVfx"); // use ice vfx instead of fire vfx
        bombvfx.transform.position = transform.position;
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
        float explosiveRadius = (BombThrowerIndex == 1) ? PlayerexplosionRadius : EnemyexplosionRadius;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            // Freeze Enemy AI
            AIMovement enemyMovement = hitCollider.GetComponentInParent<AIMovement>();
            if (enemyMovement != null && enemyMovement.PlayerIndex != BombThrowerIndex)
            {
                enemyMovement.Freeze(freezeDuration);
            }

            // Freeze Player if bomb was thrown by Enemy
            PlayerMovement playerMovement = hitCollider.GetComponentInParent<PlayerMovement>();
            if (playerMovement != null && playerMovement.PlayerIndex != BombThrowerIndex)
            {
                playerMovement.Freeze(freezeDuration); // call PlayerMovement’s own freeze function
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, EnemyexplosionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, PlayerexplosionRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (blastCorutine == null) return;

        if (collision.gameObject.CompareTag("Enemy") &&
            collision.transform.GetComponentInParent<AIMovement>().PlayerIndex != BombThrowerIndex)
        {
            StopCoroutine(blastCorutine);
            StartCoroutine(BlastAfterTime(0));
        }

        if (collision.gameObject.CompareTag("Player") &&
            collision.transform.GetComponentInParent<PlayerMovement>().PlayerIndex != BombThrowerIndex)
        {
            StopCoroutine(blastCorutine);
            StartCoroutine(BlastAfterTime(0));
        }

        if (collision.gameObject.CompareTag("Bomb") ||
            collision.gameObject.CompareTag("Impact") ||
            collision.gameObject.CompareTag("TrainingEnemy"))
        {
            StopCoroutine(blastCorutine);
            StartCoroutine(BlastAfterTime(0));
        }
    }
}
