using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotateSpeed = 5f;
    public Transform[] points;
    public Transform detectPoint;
    public float platdormDetectRadius;
    public LayerMask platformLayerMask;

    private Rigidbody rb;
    private Animator anim;
    private int currentPointIndex = 0;
    private Vector3 movementDirection;
    private float changePointCooldown = 1f;
    private bool isCheckingPlatform;
    [HideInInspector] public bool gettingDamage;
    public float knockbackTime;
    private RagdolMaintainer ragdolMaintainer;
    public int PlayerIndex;
    private Transform visibleTargets;
    private Health health;
    private PlayerMovement playerMovement;
    private FieldOfView fieldOfView;

    [Header("Bombs on Head")]
    [SerializeField] private GameObject[] headBombs; // All bomb models attached to head
    private GameObject activeBomb;                   // Currently visible bomb
    public string activeBombType;                   // Name used in pool


    private bool canThrow;
    [SerializeField] private float bombCooldownSpeed;
    [SerializeField] private GameObject HealthBar;
    private Vector3 healthbarOffset;
    [SerializeField] private Vector3 healthbarRotation;
    private float healthbarY;
    private static ObjectPooler objectPooler;
    SoundManager soundManager;
    public bool TrainingAi;

    [SerializeField] private GameObject icePrefab; // Assign in Inspector (child of enemy)



    private void OnEnable()
    {
        if (objectPooler == null && !TrainingAi)
        {
            objectPooler = ObjectPooler.instance;
        }

        InitializeWayPoints();

        // Pick a random bomb for the head
        ChooseRandomHeadBomb();

        soundManager = SoundManager.soundManager;
        Invoke(nameof(EnableBombing), Random.Range(2f, 3f));
    }

    void ChooseRandomHeadBomb()
    {
        // Hide all bombs
        foreach (var bomb in headBombs)
            bomb.SetActive(false);

        if (headBombs.Length > 0)
        {
            // Make randomness unique per AI
            Random.InitState(gameObject.GetInstanceID() + System.DateTime.Now.Millisecond);

            int randomIndex = Random.Range(0, headBombs.Length);
            activeBomb = headBombs[randomIndex];
            activeBomb.SetActive(true);

            // Store bomb type (use prefab name for pool lookup)
            activeBombType = activeBomb.name.Replace("(Clone)", "").Trim();
        }
    }



    void InitializeWayPoints()
    {
        GameObject[] wayPointsObjects = GameObject.FindGameObjectsWithTag("WayPoints");
        points = new Transform[wayPointsObjects.Length];
        for (int i = 0; i < wayPointsObjects.Length; i++)
        {
            points[i] = wayPointsObjects[i].transform;
        }
    }

    void EnableBombing()
    {
        canThrow = true;
    }

    void Start()
    {
        fieldOfView = GetComponent<FieldOfView>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        ragdolMaintainer = GetComponent<RagdolMaintainer>();
        anim = GetComponent<Animator>();
        ChooseNextPoint();
        if (!TrainingAi)
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        healthbarOffset = ragdolMaintainer.hip.transform.position - transform.position;
        healthbarY = HealthBar.transform.position.y;
    }

    void FixedUpdate()
    {
        if (!gettingDamage && health.currentHealth > 0)
        {
            CheckVisibleTargets();
            MovementAndCheckObstacle();
        }
    }

    private void LateUpdate()
    {
        UpdateHealthBarPosition();
    }

    void UpdateHealthBarPosition()
    {
        HealthBar.transform.rotation = Quaternion.Euler(healthbarRotation);
        HealthBar.transform.position = ragdolMaintainer.hip.transform.position + healthbarOffset;
        Vector3 temp = HealthBar.transform.position;
        temp.y = healthbarY;
        HealthBar.transform.position = temp;
    }

    void ChooseNextPoint()
    {
        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, points.Length);
        } while (nextIndex == currentPointIndex);

        currentPointIndex = nextIndex;
        movementDirection = (points[currentPointIndex].position - transform.position).normalized;
    }

    void MovementAndCheckObstacle()
    {
        if (Vector3.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            ChooseNextPoint();
        }

        Vector3 move = movementDirection * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        anim.SetBool("Run", true);

        movementDirection.y = 0;
        Vector3 targetDirection = visibleTargets == null
            ? Vector3.RotateTowards(transform.forward, movementDirection, rotateSpeed * Time.deltaTime, 0f)
            : Vector3.RotateTowards(transform.forward, visibleTargets.position - transform.position, rotateSpeed * Time.deltaTime, 0f);

        transform.rotation = Quaternion.LookRotation(targetDirection);

        if (!isCheckingPlatform)
        {
            StartCoroutine(CheckPlatformAhead());
        }
    }

    private IEnumerator CheckPlatformAhead()
    {
        isCheckingPlatform = true;

        if (!IsPlatformAhead())
        {
            ChooseNextPoint();
        }

        yield return new WaitForSeconds(changePointCooldown);
        isCheckingPlatform = false;
    }

    bool IsPlatformAhead()
    {
        Collider[] hitColliders = Physics.OverlapSphere(detectPoint.position, platdormDetectRadius, platformLayerMask);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }

    void CheckVisibleTargets()
    {
        visibleTargets = fieldOfView.visibleTargets.Count == 0 ? null : FindClosestVisibleTarget(fieldOfView.visibleTargets);
        if (visibleTargets != null && canThrow)
        {
            bool alreadyKnocked = visibleTargets.GetComponent<AIMovement>()?.gettingDamage ?? visibleTargets.GetComponent<PlayerMovement>()?.gettingDamage ?? true;
            if (!alreadyKnocked)
            {
                canThrow = false;
                StartCoroutine(ThrowBomb());
            }
        }
    }

    private Transform FindClosestVisibleTarget(List<Transform> visibleTargets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in visibleTargets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectPoint.position, platdormDetectRadius);
    }

    public void KnockBack()
    {
        canThrow = false;
        if (activeBomb != null) activeBomb.SetActive(false);
        gettingDamage = true;
        if (!TrainingAi)
            health.TakeDamage(playerMovement.PlayerPower);
        else
            health.TakeDamage(30);
        ragdolMaintainer.TurnOnRagdoll();
        soundManager.PlayEnemyHurtSound();
        if (health.currentHealth > 0)
        {
            StartCoroutine(RecoverFromKnockBack());
        }
        else
        {
            DisableCharacter();
        }
        HealthBar.SetActive(true);
    }

    private void DisableCharacter()
    {
        GetComponent<Target>().enabled = false;
        gameObject.layer = 0;
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    private IEnumerator RecoverFromKnockBack()
    {
        yield return new WaitForSeconds(knockbackTime);
        if (health.currentHealth > 0)
        {
            gettingDamage = false;
            ragdolMaintainer.ResetPos();
            anim.SetBool("Run", false);

            // Re-select a random bomb after recovering
            ChooseRandomHeadBomb();

            HealthBar.SetActive(false);
            canThrow = true;
        }
        else
        {
            DisableCharacter();
        }
    }

    private IEnumerator ThrowBomb()
    {
        anim.SetBool("Throw", true);

        // Get bomb from pool using the activeBombType
        GameObject currentBomb = objectPooler.GetObject(activeBombType);

        if (currentBomb != null)
        {
            currentBomb.transform.SetPositionAndRotation(activeBomb.transform.position, activeBomb.transform.rotation);

            // Hide the head bomb while throwing
            activeBomb.SetActive(false);

            yield return new WaitForEndOfFrame();

            if (visibleTargets != null)
            {
                // Safely call the correct script
                if (currentBomb.TryGetComponent<TimeBomb>(out var timeBomb))
                    timeBomb.SetData(visibleTargets.transform, PlayerIndex);
                else if (currentBomb.TryGetComponent<Bomb>(out var bomb))
                    bomb.SetData(visibleTargets.transform, PlayerIndex);
                else if (currentBomb.TryGetComponent<ClusterBomb>(out var clusterBomb))
                    clusterBomb.SetData(visibleTargets.transform, PlayerIndex);
                else if (currentBomb.TryGetComponent<FreezeBomb>(out var iceBomb))
                    iceBomb.SetData(visibleTargets.transform, PlayerIndex);
                else if (currentBomb.TryGetComponent<EnemyStickyBomb>(out var stickyBomb))
                    stickyBomb.SetData(visibleTargets.transform, PlayerIndex);

                //soundManager.PlayThrowSound();
            }
        }
        else
        {
            Debug.LogWarning("No bomb found in pool for type: " + activeBombType);
        }

        yield return new WaitForSeconds(bombCooldownSpeed);
        anim.SetBool("Throw", false);

        if (!gettingDamage)
        {
            // Pick another random bomb for head
            ChooseRandomHeadBomb();
            canThrow = true;
        }
    }


    public void DisableGameObject()
    {
        StartCoroutine(DisableAfterTime());
    }

    public void Freeze(float freezeDuration)
    {
        if (gettingDamage) return;

        gettingDamage = true;

        // Stop movement completely
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        anim.enabled = false;

        // Activate ice prefab
        if (icePrefab != null)
            icePrefab.SetActive(true);

        // Unfreeze after duration
        StartCoroutine(UnfreezeAfterDelay(freezeDuration));
    }


    private IEnumerator UnfreezeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rb.isKinematic = false;
        gettingDamage = false;
        anim.enabled = true;
        anim.SetBool("Run", true);

        // Hide ice prefab
        if (icePrefab != null)
            icePrefab.SetActive(false);

        if (activeBomb == null || !activeBomb.activeSelf)
        {
            ChooseRandomHeadBomb();
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("DeadZone"))
    //    {
    //        DieInDeadZone();
    //    }
    //}

    //private void DieInDeadZone()
    //{
    //    health.currentHealth = 0;
    //    // Prevent further logic
    //    gettingDamage = true;
    //    canThrow = false;

    //    // Disable after short delay (falling looks natural)
    //    StartCoroutine(DisableAfterTime());
    //}

}
