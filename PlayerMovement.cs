using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] public HandAnimator handAnimator;
    [SerializeField] Stamina stamina;
    public float sprintDrainPerSec = 10f;

    public float walkingSpeed = 10f;
    public float runningSpeed = 15f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float accelTime = 0.12f;
    private float speedVel;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    private bool isRunning;
    private bool canRun = true;

    private Vector3 moveInput;
    private bool jumpPressed;

    private Vector3 lastPosition;

    private float currentSpeed;
    private float moveAmount;
    private float moveAmountVel;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 시작 속도 기본값
        currentSpeed = 0f;
        moveAmount = 0f;
    }

    void Update()
    {
        // Ground 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // 입력 기반 이동 판단(더 안정적)
        float inputAmount = Mathf.Clamp01(moveInput.magnitude);
        isMoving = inputAmount > 0.01f;

        //shouldRun 기준으로 달리기 판단
        bool shouldRun = isRunning && isMoving && isGrounded;

        if (shouldRun)
        {
            float cost = sprintDrainPerSec * Time.deltaTime;
            bool ok = stamina != null && stamina.Spend(cost);
            if (!ok)
            {
                isRunning = false;
                shouldRun = false;
            }
        }

        float targetSpeed = 0f;
        if (isMoving)
            targetSpeed = shouldRun ? runningSpeed : walkingSpeed;
        //속도 보간
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVel, accelTime);

        // 이동 방향(대각선 과속 방지)
        Vector3 dir = transform.right * moveInput.x + transform.forward * moveInput.z;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        controller.Move(dir * currentSpeed * Time.deltaTime);

        // 점프
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 애니메이션 파라미터
        handAnimator.animator.SetBool(HandAnimeParams.isMoving, isMoving);

        //블렌드 트리 계산
        float targetMoveAmount = 0f;
        if (isMoving)
            targetMoveAmount = isRunning ? 1f : 0f;

        moveAmount = Mathf.SmoothDamp(moveAmount, targetMoveAmount, ref moveAmountVel, accelTime);

        //애니 파라미터
        handAnimator.animator.SetFloat(HandAnimeParams.Speed, moveAmount);

        // (원래 쓰던 lastPosition은 유지해둠. 필요 없으면 지워도 됨)
        lastPosition = transform.position;
    }
    void HandleRun()
    {
        if (!isRunning)
            return;
        float cost = sprintDrainPerSec * Time.deltaTime;
        bool ok = stamina != null && stamina.Spend(cost);
        if(!ok)
        {
            isRunning = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = new Vector3(input.x, 0, input.y);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (canRun)
        {
            isRunning = context.ReadValueAsButton();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
            jumpPressed = true;
    }
    public void SetCanRun(bool value)
    {
        canRun = value;
        if (!canRun) isRunning = false;
    }

}
