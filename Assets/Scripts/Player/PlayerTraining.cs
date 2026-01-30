using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerTraining : MonoBehaviour
{
    private Joystick joystick;
    private Animator anim;
    private Rigidbody rb;
    private SoundManager soundManager;
    private LineRenderer lineRenderer;

    Vector3 movementDirection;

    public float movementSpeed = 5f, rotateSpeed = 5f;
    public float movementTime = 5f;
    public float bombCooldownSpeed;
    public float h, gravity;

    public int trajectoryResolution;
    public int totalTarget;

    int currentTarget;
    float movementwait;

    bool hasMoved, canThrow, AimCanvasShown, MoveCanvasShowed;

    public GameObject myBomb;
    public GameObject TrainingCompleteCanvas;

    [SerializeField] GameObject MoveCanvas, AimCanvas;
    [SerializeField] GameObject Aimer;
    [SerializeField] Transform defaultThrowPos;
    [SerializeField] GameObject TrainingAlreadyComplete;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = 0;
        lineRenderer = GetComponentInChildren<LineRenderer>();
        soundManager = SoundManager.soundManager;
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.FindGameObjectWithTag("Joystick").transform.GetChild(0).GetComponent<Joystick>();
        anim = GetComponent<Animator>();
        anim.SetLayerWeight(1, 0f);
        Application.targetFrameRate = 60;

        if (PlayerPrefs.GetInt("TrainingComplete") == 1)
        {
            TrainingAlreadyComplete.SetActive(true);
            //SkipTraining();
        }
    }

    private void FixedUpdate()
    {
        MovementAndThrowBomb();
    }

    private void MovementAndThrowBomb()
    {
        movementDirection = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);

        if (movementDirection.sqrMagnitude > 0f)
        {
            MoveCanvas.SetActive(false);
            movementDirection.Normalize();

            Vector3 move = movementDirection * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            anim.SetBool("Run", true);

            Vector3 targetDirection;
            targetDirection = Vector3.RotateTowards(
                new Vector3(transform.forward.x, 0, transform.forward.z),
                new Vector3(movementDirection.x, 0, movementDirection.z),
                rotateSpeed * Time.deltaTime,
                0f
            );

            transform.rotation = Quaternion.LookRotation(targetDirection);

            if (movementwait < movementTime)
                movementwait += Time.deltaTime;
            else
            {
                if (!hasMoved)
                {
                    SetBombNow();
                    hasMoved = true;
                }
            }

            if (AimCanvasShown)
                AimCanvas.SetActive(false);
        }
        //else if (hasMoved && canThrow)
        //{
        //    StartCoroutine(ThrowBomb());
        //    canThrow = false;
        //}
        else
        {
            anim.SetBool("Run", false);

            if (!hasMoved && !MoveCanvasShowed)
                MoveCanvas.SetActive(true);
            else
            {
                AimCanvas.SetActive(true);
                AimCanvasShown = true;
            }
        }
    }

    void SetBombNow()
    {
        anim.SetLayerWeight(1, 1f);
        myBomb.SetActive(true);
        MoveCanvas.SetActive(false);

        Aimer.transform.GetChild(0).gameObject.SetActive(true);

        ParticleSystem particleSystem = Aimer.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.loop = true;
        main.playOnAwake = true;
        particleSystem.Play();

        soundManager = SoundManager.soundManager;

        AimCanvas.SetActive(true);
        canThrow = true;
        hasMoved = true;
        MoveCanvasShowed = true;
    }

    private IEnumerator ThrowBomb()
    {
        anim.SetBool("Throw", true);

        GameObject currentBomb = ObjectPooler.instance.GetObject("Bomb");
        currentBomb.transform.SetPositionAndRotation(myBomb.transform.position, myBomb.transform.rotation);

        lineRenderer.positionCount = 0;
        myBomb.SetActive(false);

        yield return new WaitForEndOfFrame();

        currentBomb.GetComponent<Bomb>().SetData(Aimer.transform, 1);
        soundManager.PlayThrowSound();

        yield return new WaitForSeconds(bombCooldownSpeed);

        anim.SetBool("Throw", false);
        hasMoved = false;
    }

    private void Update()
    {
        CalculateAndDrawTrajectory();
    }

    private void CalculateAndDrawTrajectory()
    {
        Vector3 aimPos = new Vector3(defaultThrowPos.position.x, 0.1f, defaultThrowPos.position.z);
        Aimer.transform.position = aimPos;

        if (!canThrow)
        {
            Aimer.gameObject.SetActive(false);
            return;
        }

        Aimer.gameObject.SetActive(true);

        Vector3[] trajectoryPoints = CalculateTrajectoryPoints(myBomb.transform.position, aimPos, h, gravity, trajectoryResolution);
        lineRenderer.positionCount = trajectoryPoints.Length;
        lineRenderer.SetPositions(trajectoryPoints);
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

    public void TargetDown()
    {
        currentTarget++;
        if (currentTarget >= totalTarget)
        {
            StartCoroutine(ShowTrainingComplete());
        }
    }

    IEnumerator ShowTrainingComplete()
    {
        yield return new WaitForSeconds(1f);
        TrainingCompleteCanvas.SetActive(true);
        TrainingCompleteCanvas.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
        PlayerPrefs.SetInt("TrainingComplete", 1);
    }

    public void SkipTraining()
    {
        soundManager.PlayButtonSound();
        SceneManager.LoadScene("Hexagon");
    }

    public void Retry()
    {
        soundManager.PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseTrainingCompleteBox()
    {
        soundManager.PlayButtonSound();
        TrainingAlreadyComplete.SetActive(false);
    }

    public void OnThrowButtonPressed()
    {
        if (canThrow && hasMoved)
        {
            StartCoroutine(ThrowBomb());
            canThrow = false;
        }
    }
}
