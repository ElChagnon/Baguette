using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class CharacterController : MonoBehaviour
{
    public int playerNumber = 1;
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;
    private bool isRunning = false;
    public bool isGrounded = false;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private string horizontalAxis;
    private string verticalAxis;
    private KeyCode jumpKey;
    private KeyCode runKey;

    private static string[] joystickNames;
    private const float deadZone = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the player.");
        }

        joystickNames = Input.GetJoystickNames();
        AssignController(playerNumber - 1);
    }

    private void AssignController(int joystickIndex)
    {
        if (joystickIndex >= joystickNames.Length || string.IsNullOrEmpty(joystickNames[joystickIndex]))
        {
            Debug.LogError("No controller detected for player " + playerNumber + ". Please check connections.");
            return;
        }

        Debug.Log("Controller assigned to player " + playerNumber + ": " + joystickNames[joystickIndex]);

        horizontalAxis = "Joystick" + (joystickIndex + 1) + "AxisX";
        verticalAxis = "Joystick" + (joystickIndex + 1) + "AxisY";
        jumpKey = (KeyCode)((int)KeyCode.Joystick1Button0 + joystickIndex * 20);
        runKey = (KeyCode)((int)KeyCode.Joystick1Button5 + joystickIndex * 20);
    }

    private void Update()
    {
        GroundCheck();
        HandleMovement();
        HandleJump();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.1f, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis(horizontalAxis);
        float vertical = Input.GetAxis(verticalAxis);

        if (Mathf.Abs(horizontal) < deadZone) horizontal = 0;
        if (Mathf.Abs(vertical) < deadZone) vertical = 0;

        isRunning = Input.GetKey(runKey) || Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

            float targetAngle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            targetAngle = -targetAngle;

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            animator.SetBool("isWalking", !isRunning);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) || Input.GetKeyDown(KeyCode.Space))
        {
            lastJumpPressedTime = Time.time;
        }

        if ((Time.time - lastJumpPressedTime <= jumpBufferTime) && (Time.time - lastGroundedTime <= coyoteTime))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            animator.SetTrigger("Jump");
            lastJumpPressedTime = float.MinValue;
        }
    }
}