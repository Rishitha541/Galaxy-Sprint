using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Lane Movement")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float laneChangeSpeed = 12f;
    [SerializeField] private int startLane = 1;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float gravity = -28f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.75f;
    [SerializeField] private float slideHeight = 0.95f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CharacterController characterController;
    private Vector3 initialPosition;
    private float verticalVelocity;
    private int targetLane;
    private bool isSliding;
    private bool isDead;
    private float originalControllerHeight;
    private Vector3 originalControllerCenter;
    private Coroutine slideRoutine;

    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int SlideHash = Animator.StringToHash("Slide");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        initialPosition = transform.position;
        targetLane = Mathf.Clamp(startLane, 0, 2);

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        originalControllerHeight = characterController.height;
        originalControllerCenter = characterController.center;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted += ResetPlayer;
            GameManager.Instance.OnGameOver += PlayDeath;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= ResetPlayer;
            GameManager.Instance.OnGameOver -= PlayDeath;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying || isDead)
        {
            SetRunning(false);
            return;
        }

        SetRunning(true);
        HandleLaneInput();
        HandleJumpInput();
        HandleSlideInput();
        MovePlayer();
    }

    private void HandleLaneInput()
    {
        bool moveLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        bool moveRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        if (moveLeft)
        {
            targetLane = Mathf.Max(0, targetLane - 1);
        }
        else if (moveRight)
        {
            targetLane = Mathf.Min(2, targetLane + 1);
        }
    }

    private void HandleJumpInput()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
            animator?.SetTrigger(JumpHash);
            AudioManager.Instance?.PlayJump();
        }
    }

    private void HandleSlideInput()
    {
        bool slidePressed = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)
    || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        if (!slidePressed || isSliding || !characterController.isGrounded)
        {
            return;
        }

        slideRoutine = StartCoroutine(SlideRoutine());
    }

    private void MovePlayer()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        float targetX = (targetLane - 1) * laneDistance;
        float nextX = Mathf.Lerp(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        float horizontalDelta = nextX - transform.position.x;
        float forwardDelta = GameManager.Instance.CurrentSpeed * Time.deltaTime;
        float verticalDelta = verticalVelocity * Time.deltaTime;

        Vector3 movement = new Vector3(horizontalDelta, verticalDelta, forwardDelta);
        characterController.Move(movement);
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        animator?.SetTrigger(SlideHash);

        characterController.height = slideHeight;
        characterController.center = new Vector3(
            originalControllerCenter.x,
            slideHeight * 0.5f,
            originalControllerCenter.z
        );

        yield return new WaitForSeconds(slideDuration);

        characterController.height = originalControllerHeight;
        characterController.center = originalControllerCenter;
        isSliding = false;
        slideRoutine = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (hit.collider.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    private void ResetPlayer()
    {
        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
        }

        isDead = false;
        isSliding = false;
        verticalVelocity = 0f;
        targetLane = Mathf.Clamp(startLane, 0, 2);

        characterController.enabled = false;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        characterController.height = originalControllerHeight;
        characterController.center = originalControllerCenter;
        characterController.enabled = true;

        animator?.ResetTrigger(DeathHash);
        SetRunning(true);
    }

    private void PlayDeath()
    {
        isDead = true;
        SetRunning(false);
        animator?.SetTrigger(DeathHash);
    }

    private void SetRunning(bool isRunning)
    {
        animator?.SetBool(IsRunningHash, isRunning);
    }
}
