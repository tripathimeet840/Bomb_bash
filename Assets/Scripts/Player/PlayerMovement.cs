using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick;
    private Animator anim;
    private Rigidbody rb;

    public float movementSpeed = 5f;
    public float rotateSpeed = 720f;
    public float bombCooldownSpeed = 1f;

    private bool gameStarted = false;
    public bool hasMoved = false;
    private bool canThrow = false;
    [HideInInspector] public bool gettingDamage = false;
    public GameObject myBomb;

    //public GameObject bombPrefab;
    [SerializeField] private Transform defaultThrowPos;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform Aimer;
    [SerializeField] private GameObject ThrowButton;

    public float h = 2f;
    public float gravity = -20f;
    public int trajectoryResolution;

    private Vector3 movementDirection;
    private RagdolMaintainer ragdolMaintainer;
    public float knockbackTime;
    private FieldOfView fieldOfView;
    public int PlayerIndex = 1;
    public float PlayerPower, EnemyPower;
    private Health health;
    [SerializeField] private GameObject HealthBar;
    private Vector3 healthbarOffset;
    [SerializeField] private Vector3 healthbarRotation;
    private float healthbarY;
    CinemachineVirtualCamera followcam, wonCam;
    public GameObject followCamera;
    public GameObject wonCamera;
    private bool won;
    private GameObject confetti;
    private SoundManager soundManager;
    public bool dead;
    PlayerBombsHandler playerBombsHandler;

    public BombCollector bombCollector;

    public GameObject normalBomb;
    public GameObject clusterBomb;
    public GameObject timeBomb;
    public GameObject iceBomb;
    public GameObject stickyBomb;

    private StickyBomb activeStickyBomb; 

    [SerializeField] private GameObject icePrefab; 

    [Header("Rewards")]
    public int xpRewardOnWin = 200;

    public GameObject losePanel;
    //public GameObject blastButton;

    private void Start()
    {
        losePanel.SetActive(false);
        gameStarted = true;
        followCamera.SetActive(true);
        wonCamera.SetActive(false);

        Aimer = GameManager.instance.Aimer;
        health = GetComponent<Health>();
        bombCollector = GetComponent<BombCollector>();
        fieldOfView = GetComponent<FieldOfView>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        ragdolMaintainer = GetComponent<RagdolMaintainer>();
        soundManager = SoundManager.soundManager;

        healthbarOffset = ragdolMaintainer.hip.transform.position - transform.position;
        healthbarY = HealthBar.transform.position.y;

        confetti = GameObject.FindGameObjectWithTag("Confetti");

        anim = GetComponent<Animator>();
        anim.SetLayerWeight(1, 0f);
        if (GameManager.instance != null)
        {
            joystick = GameManager.instance.joystick;
        }
        else
        {
            Debug.LogError("GameManager instance is null. Joystick cannot be assigned.");
        }
        if (joystick != null)
            joystick.gameObject.SetActive(true);
        ThrowButton.SetActive(true);

        followcam = GameObject.FindGameObjectWithTag("FollowCam").GetComponent<CinemachineVirtualCamera>();
        followcam.Follow = ragdolMaintainer.hip;

        wonCam = GameObject.FindGameObjectWithTag("WonCam").GetComponent<CinemachineVirtualCamera>();
        wonCam.Follow = ragdolMaintainer.hip;
        wonCam.LookAt = transform;

        playerBombsHandler = GetComponent<PlayerBombsHandler>();
        myBomb = playerBombsHandler.allBombs[PlayerPrefs.GetInt("BombSkin", 0)].BombVisual;

        CalculateAndDrawTrajectory();
    }

    public void DisableControls()
    {

        joystick.gameObject.SetActive(false);
        ThrowButton.SetActive(false);
    }

    public void StartGame()
    {
        if (gameStarted) return;

        if (Aimer != null)
            Aimer.transform.GetChild(0).gameObject.SetActive(false);
        myBomb = playerBombsHandler.allBombs[PlayerPrefs.GetInt("BombSkin", 0)].BombVisual;

        ParticleSystem particleSystem = Aimer.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.loop = true;
        main.playOnAwake = true;
        particleSystem.Play();
        soundManager = SoundManager.soundManager;
        joystick.gameObject.SetActive(true);
        ThrowButton.SetActive(true);

        canThrow = true;
        gameStarted = true;

        //anim.SetLayerWeight(1, 1f);
        myBomb.SetActive(false);
        followcam = GameObject.FindGameObjectWithTag("FollowCam").GetComponent<CinemachineVirtualCamera>();
        followcam.Follow = ragdolMaintainer.hip;
    }

    private void Update()
    {
        if (won || !gameStarted) return;

        if (bombCollector.bombCount > 0 && health.currentHealth > 0)
        {
            myBomb.SetActive(true);
            anim.SetLayerWeight(1, 1f);
            Aimer.gameObject.SetActive(true);
            UpdateHeldBombVisual();

            canThrow = true;
        }
        else
        {
            myBomb.SetActive(false);
            anim.SetLayerWeight(1, 0f);
            Aimer.gameObject.SetActive(false);
        }

        if (!gettingDamage && health.currentHealth > 0 && bombCollector.bombCount > 0)
            CalculateAndDrawTrajectory();
        if (health.currentHealth <= 0)
            lineRenderer.positionCount = 0;

        if (health.currentHealth <= 0)
        {
            DisableControls();
            lineRenderer.positionCount = 0;
            Aimer.gameObject.SetActive(false);
        }

        if (health.currentHealth >= 0)
        {
            HealthBar.SetActive(true);
        }

        //if (gameStarted)
        //{
        //    joystick.gameObject.SetActive(true);    
        //    ThrowButton.gameObject.SetActive(true);
        //}
    }

    private void UpdateHeldBombVisual()
    {
        normalBomb.SetActive(false);
        timeBomb.SetActive(false);
        clusterBomb.SetActive(false);
        iceBomb.SetActive(false);
        stickyBomb.SetActive(false);

        myBomb = null;

        if (bombCollector.currentBombType == "CollectableBomb")
        {
            myBomb = normalBomb;
        }
        else if (bombCollector.currentBombType == "CollectableTimeBomb")
        {
            myBomb = timeBomb;
        }
        else if (bombCollector.currentBombType == "CollectableClusterBomb")
        {
            myBomb = clusterBomb;
        }
        else if (bombCollector.currentBombType == "CollectableIceBomb")
        {
            myBomb = iceBomb;
        }
        else if (bombCollector.currentBombType == "CollectableStickyBomb")
        {
            myBomb = stickyBomb;
        }

        if (myBomb != null && bombCollector.bombCount > 0)
            myBomb.SetActive(true);
    }

    private void LateUpdate()
    {
        if (!gameStarted) return;

        HealthBar.transform.rotation = Quaternion.Euler(healthbarRotation);
        HealthBar.transform.position = ragdolMaintainer.hip.transform.position + healthbarOffset;
        Vector3 temp = HealthBar.transform.position;
        temp.y = healthbarY;
        HealthBar.transform.position = temp;
    }

    private void FixedUpdate()
    {
        if (won || !gameStarted || gettingDamage || health.currentHealth <= 0) return;

        MovementAndThrowBomb();
    }

    private void MovementAndThrowBomb()
    {
        movementDirection = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);

        if (movementDirection.sqrMagnitude > 0f)
        {
            movementDirection.Normalize();
            Vector3 move = movementDirection * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            hasMoved = true;
            anim.SetBool("Run", true);

            Vector3 targetDirection;
            if (fieldOfView.visibleTargets.Count == 0)
            {
                movementDirection.y = 0;
                targetDirection = Vector3.RotateTowards(
                    new Vector3(transform.forward.x, 0, transform.forward.z),
                    new Vector3(movementDirection.x, 0, movementDirection.z),
                    rotateSpeed * Time.deltaTime,
                    0f
                );
            }
            else
            {
                Transform target = fieldOfView.visibleTargets[0];

                if (!target.GetComponent<AIMovement>().gettingDamage)
                {
                    Vector3 directionToTarget = target.position - transform.position;
                    directionToTarget.y = 0;
                    targetDirection = Vector3.RotateTowards(
                        new Vector3(transform.forward.x, 0, transform.forward.z),
                        new Vector3(directionToTarget.x, 0, directionToTarget.z),
                        rotateSpeed * Time.deltaTime,
                        0f
                    );
                }
                else
                {
                    movementDirection.y = 0;
                    targetDirection = Vector3.RotateTowards(
                        new Vector3(transform.forward.x, 0, transform.forward.z),
                        new Vector3(movementDirection.x, 0, movementDirection.z),
                        rotateSpeed * Time.deltaTime,
                        0f
                    );
                }
            }

            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
        //else if (hasMoved && canThrow && bombCollector.bombCount > 0)
        //{
        //    StartCoroutine(ThrowBomb());
        //    canThrow = false;
        //}
        else
        {
            anim.SetBool("Run", false);
        }
    }

    private IEnumerator ThrowBomb()
    {
        anim.SetBool("Throw", true);

        string bombPrefabName = "";
        if (bombCollector.currentBombType == "CollectableTimeBomb")
            bombPrefabName = "TimeBomb";
        else if (bombCollector.currentBombType == "CollectableClusterBomb")
            bombPrefabName = "ClusterBomb";
        else if (bombCollector.currentBombType == "CollectableBomb")
            bombPrefabName = "NormalBomb";
        else if (bombCollector.currentBombType == "CollectableIceBomb")
            bombPrefabName = "FreezeBomb";
        else if (bombCollector.currentBombType == "CollectableStickyBomb")
            bombPrefabName = "StickyBomb";

        GameObject currentBomb = ObjectPooler.instance.GetObject(bombPrefabName);

        bombCollector.bombCount--;
        bombCollector.currentBombType = "";
        bombCollector.SendMessage("UpdateBombState");

        currentBomb.transform.SetPositionAndRotation(myBomb.transform.position, myBomb.transform.rotation);
        lineRenderer.positionCount = 0;
        myBomb.SetActive(false);

        yield return new WaitForEndOfFrame();

        if (currentBomb.TryGetComponent<TimeBomb>(out var timeBomb))
            timeBomb.SetData(Aimer.transform, PlayerIndex);
        else if (currentBomb.TryGetComponent<Bomb>(out var normalBomb))
            normalBomb.SetData(Aimer.transform, PlayerIndex);
        else if (currentBomb.TryGetComponent<ClusterBomb>(out var clusterBomb))
            clusterBomb.SetData(Aimer.transform, PlayerIndex);
        else if (currentBomb.TryGetComponent<FreezeBomb>(out var iceBomb))
            iceBomb.SetData(Aimer.transform, PlayerIndex);
        else if (currentBomb.TryGetComponent<StickyBomb>(out var stickyBomb))
        {
            stickyBomb.SetData(Aimer.transform, PlayerIndex);
            activeStickyBomb = stickyBomb;
        }

        soundManager.PlayThrowSound();

        yield return new WaitForSeconds(bombCooldownSpeed);
        anim.SetBool("Throw", false);

        hasMoved = false;

        if (!gettingDamage && !won && bombCollector.bombCount > 0)
        {
            myBomb.SetActive(true);
            canThrow = true;
            Aimer.gameObject.SetActive(true);
        }
        else
        {
            myBomb.SetActive(false);
            canThrow = false;
            Aimer.gameObject.SetActive(false);
        }
    }



    private void CalculateAndDrawTrajectory()
    {
        Vector3 aimPos = new Vector3(defaultThrowPos.position.x, 0.1f, defaultThrowPos.position.z);
        if (fieldOfView.visibleTargets.Count != 0)
        {
            Transform closestEnemy = FindClosestVisibleTarget(fieldOfView.visibleTargets);
            if (closestEnemy != null)
            {
                Vector3 enemyPos = closestEnemy.GetComponent<RagdolMaintainer>().hip.position;
                aimPos = new Vector3(enemyPos.x, 0.1f, enemyPos.z);
            }
        }
        Aimer.transform.position = aimPos;
        if (!gameStarted || !canThrow)
        {
            Aimer.gameObject.SetActive(false);
            return;
        }

        Aimer.gameObject.SetActive(true);

        Vector3[] trajectoryPoints = CalculateTrajectoryPoints(myBomb.transform.position, aimPos, h, gravity, trajectoryResolution);
        foreach (Vector3 point in trajectoryPoints)
        {
            if (float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z))
            {
                lineRenderer.positionCount = 0;
                return;
            }
        }

        if (health.currentHealth > 0)
        {
            lineRenderer.positionCount = trajectoryPoints.Length;
            lineRenderer.SetPositions(trajectoryPoints);
        }
        else
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }

    private Transform FindClosestVisibleTarget(List<Transform> visibleTargets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in visibleTargets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance && !target.GetComponent<AIMovement>().gettingDamage)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    private Vector3[] CalculateTrajectoryPoints(Vector3 start, Vector3 end, float height, float gravity, int resolution)
    {
        Vector3[] points = new Vector3[resolution];
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;
        Vector3 initialVelocity = velocityXZ + velocityY * -Mathf.Sign(gravity);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1) * time;
            points[i] = start + initialVelocity * t + 0.5f * Vector3.up * gravity * t * t;
        }

        return points;
    }

    public void KnockBack()
    {
        if (won) return;
        soundManager.PlayPlayerHurt();
        myBomb.SetActive(false);
        lineRenderer.positionCount = 0;
        gettingDamage = true;
        Aimer.gameObject.SetActive(false);
        ragdolMaintainer.TurnOnRagdoll();
        StartCoroutine(RecoverFromKnockBack());
        HealthBar.SetActive(true);
    }

    private IEnumerator RecoverFromKnockBack()
    {
        yield return new WaitForSeconds(knockbackTime);
        if (health.currentHealth > 0)
        {
            gettingDamage = false;
            ragdolMaintainer.ResetPos();
            anim.SetBool("Run", false);
            myBomb.SetActive(true);
            hasMoved = false;
            canThrow = true;
            gettingDamage = false;
            HealthBar.SetActive(false);
        }
    }

    public void Won()
    {
        won = true;

        int currentXP = PlayerPrefs.GetInt("PendingXP", 0);
        currentXP += xpRewardOnWin;
        PlayerPrefs.SetInt("PendingXP", currentXP);

        StartCoroutine(WaitWonTransition());
    }


    private IEnumerator WaitWonTransition()
    {
        anim.SetTrigger("Win");
        followCamera.SetActive(false);
        wonCamera.SetActive(true);

        yield return new WaitForSeconds(1f);
        soundManager.PlayLevelClear();
        canThrow = false;
        gameStarted = false;
        DisableControls();
        lineRenderer.positionCount = 0;
        anim.SetLayerWeight(1, 0f);
        wonCam.Priority = 15;
        ragdolMaintainer.TurnOffRagdoll();
        rb.isKinematic = true;
        anim.SetBool("Run", false);
        anim.SetBool("Throw", false);
        HealthBar.SetActive(false);
        myBomb.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        wonCam.Follow = null;
        wonCam.LookAt = null;
        confetti.transform.position = new Vector3(transform.position.x, 10f, transform.position.z);
        confetti.GetComponent<ParticleSystem>().Play();
        joystick.gameObject.SetActive(false);
    }

    public void OnThrowButtonPressed()
    {
        if (canThrow && bombCollector.bombCount > 0 && !gettingDamage && !won)
        {
            StartCoroutine(ThrowBomb());
            canThrow = false;
        }
    }

    public void Freeze(float freezeDuration)
    {
        if (gettingDamage) return;

        if (icePrefab != null)
            icePrefab.SetActive(true);

        gettingDamage = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        anim.enabled = false;

        joystick.gameObject.SetActive(false);
        ThrowButton.SetActive(false);

        StartCoroutine(UnfreezeAfterDelay(freezeDuration));
    }

    private IEnumerator UnfreezeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (icePrefab != null)
            icePrefab.SetActive(false);

        rb.isKinematic = false;
        gettingDamage = false;

        anim.enabled = true;

        joystick.gameObject.SetActive(true);
        ThrowButton.SetActive(true);

        joystick.OnPointerUp(null);
    }

    public void RevivePlayer()
    {
        if (!dead) return;

        losePanel.SetActive(false);

        // Reset health
        health.currentHealth = health.maxHealth;
        dead = false;
        gettingDamage = false;
        won = false;

        // Reset ragdoll & physics
        ragdolMaintainer.ResetPos();
        ragdolMaintainer.TurnOffRagdoll();
        rb.isKinematic = false;

        // Reset animations
        anim.enabled = true;
        anim.SetBool("Run", false);
        anim.SetBool("Throw", false);
        anim.SetLayerWeight(1, 0f);

        // Reactivate UI & controls
        joystick.gameObject.SetActive(true);
        ThrowButton.SetActive(true);
        HealthBar.SetActive(true);

        // Reset bombs if needed
        if (bombCollector.bombCount > 0)
            myBomb.SetActive(true);

        transform.position = Vector3.zero;

        // Clear trajectory
        lineRenderer.positionCount = 0;

        // Reset health bar slider if assigned
        if (health.healthBar != null)
            health.healthBar.fillAmount = 1f;

        // Reset joystick input
        if (joystick != null)
            joystick.OnPointerUp(null); // This clears any current input

        // Reset alreadyDied so player can die again
        health.alreadyDied = false;

        Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.Revive);
        Debug.Log("Player Revived!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone") && !dead)
        {
            DieFromFall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (health.currentHealth < health.maxHealth && collision.collider.CompareTag("HealCrate"))
        {
            health.HealthInc(30);
            Destroy(collision.gameObject);
        }
    }

    private void DieFromFall()
    {
        dead = true;
        gettingDamage = true;
        health.currentHealth = 0;
        ragdolMaintainer.TurnOnRagdoll();
        DisableControls();
        losePanel.SetActive(true);

        Debug.Log("Player died from falling off!");
    }



    //public void DetonateStickyBomb()
    //{
    //    if (activeStickyBomb != null)
    //    {
    //        activeStickyBomb.Detonate(); // Call the bomb's detonate method
    //        activeStickyBomb = null;     // Clear the reference so you can throw a new bomb
    //    }
    //}

}
