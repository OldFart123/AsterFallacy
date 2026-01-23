using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Misc.")]
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private TrailRenderer Trails;

    private Rigidbody2D rigid_bod;
    private SpriteRenderer sprite_renderer;
    private Animator animator;
    private BoxCollider2D BoxColli;

    [Header("Movement")]
    public float SpeedMove = 7f;
    public float JumpPower = 7.76f;
    private float moving_X;

    [Header("Dashing")]
    private bool CanDash = true;
    private bool IsDashing;
    public float Dashing_Power = 10f;
    public float DashingTime = 0.2f;
    public float DashingCooldown = 1f;

    [Header("Sprinting")]
    public float SprintSpeed = 10f;
    public float SprintHoldTime = 0.2f;
    private float shiftHeldTimer;
    private float baseMoveSpeed;

    [Header("WallSliding and Jumping")]
    private bool IsWallSliding;
    private float WallSlidingSpeed = 3f;
    private float WallSlideTimer;
    private float WallClingTimer;
    private bool IsWallClinging;

    private bool IsWallJumping;
    private float WallJumpingDirection;
    private float WallJumpingTime = 0.3f;
    private float WallJumpingCounter;
    private float WallJumpingDuration = 0.2f;
    public Vector2 WallJumping_Power = new Vector2(2f, 7f);

    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float wallSlideDelay = 0.08f;
    [SerializeField] private float wallClingTime = 0.3f;
    [SerializeField] private float wallClingFallSpeed = 0f;

    [Header("Jump Height")]
    [Range(0.1f, 1f)]
    public float JumpCutMultiplier = 0.6f;

    [Header("Jump Momentum")]
    public float JumpForwardForce = 3f;
    public float SprintJumpMultiplier = 1.5f;
    public float JumpAcceleration = 20f;


    [Header("Gravity Control")]
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    private bool facingRight = true;
    private bool isGrounded;
    private float playerHalfHeight;

    [Header("Ledge Grab")]
    [SerializeField] private float ledgeCheckHeight = 0.4f;
    [SerializeField] private float ledgeClimbUp = 0.6f;

    [Header("Air Control")]
    [SerializeField] private float airAcceleration = 15f;
    [SerializeField] private float airDeceleration = 10f;
    [SerializeField] private float airMaxSpeed;

    private bool IsLedgeGrabbing;
    private Vector2 ledgePos;
    private float originalGravity;
    private float jumpMomentum;

    //public GameObject AttackPoint;
    //public float radius;
    //public LayerMask Enemies;
    //public float damage_to_enemies;

    [Header("SFX")]
    [SerializeField] private AudioClip JumpSound;

    public bool IsWallClinging1 { get => IsWallClinging; set => IsWallClinging = value; }

    //SoundManager.instance.PlaySound(JumpSound);

    private void Awake()
    {
        rigid_bod = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        BoxColli = GetComponent<BoxCollider2D>();

        baseMoveSpeed = SpeedMove;
        airMaxSpeed = SprintSpeed * 1.1f;
        playerHalfHeight = sprite_renderer.bounds.extents.y;
    }

    void Update()
    {
        moving_X = Input.GetAxis("Horizontal");
        isGrounded = CheckGrounded();

        if (IsLedgeGrabbing)
        {
            HandleLedgeGrab();
            return;
        }

        if (IsDashing)
        {
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool("Kick", true);
        }

        HandleJumpInput();
        HandleDashOrSprint();
        HandleMovement();
        HandleWallSlide();
        HandleWallJump();

        animator.SetFloat("IsRunning", Mathf.Abs(rigid_bod.linearVelocity.x));
        animator.SetFloat("Grounded", rigid_bod.linearVelocity.y);
        animator.SetBool("IsJUMPing", !isGrounded);
        animator.SetBool("IsWallSliding", IsWallSliding);
        animator.SetBool("IsWallClinging", IsWallClinging = WallClingTimer < wallClingTime && WallClingTimer > 0f);
    }

    void FixedUpdate()
    {
        if (IsDashing || IsLedgeGrabbing)
        {
            return;
        }

        float targetSpeed = moving_X * SpeedMove;
        float speedDiff = targetSpeed - rigid_bod.linearVelocity.x;

        float accelRate;

        if (isGrounded)
        {
            accelRate = SpeedMove * 10f;
        }
        else
        {
            accelRate = Mathf.Abs(targetSpeed) > 0.01f ? airAcceleration : airDeceleration;
        }

        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        if (!isGrounded && Mathf.Abs(rigid_bod.linearVelocity.x) > airMaxSpeed && Mathf.Sign(rigid_bod.linearVelocity.x) == Mathf.Sign(moving_X))
        {
            movement = 0;
        }

        rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x + movement, rigid_bod.linearVelocity.y);

        ApplyBetterGravity();

        if (isGrounded)
        {
            ResetWallState();
        }
    }



    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, JumpPower);
        }

        if (Input.GetButtonUp("Jump") && rigid_bod.linearVelocity.y > 0)
        {
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, rigid_bod.linearVelocity.y * JumpCutMultiplier);
        }
    }


    private void HandleDashOrSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (isGrounded && !IsDashing)
            {
                shiftHeldTimer += Time.deltaTime;

                if (shiftHeldTimer >= SprintHoldTime)
                {
                    SpeedMove = SprintSpeed;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (shiftHeldTimer < SprintHoldTime && CanDash)
            {
                StartCoroutine(Dash());
            }
            ResetSprint();
        }

        if (!isGrounded)
        {
            ResetSprint();
        }
    }
    private void ResetSprint()
    {
        shiftHeldTimer = 0f;
        SpeedMove = baseMoveSpeed;
    }

    private void HandleMovement()
    {
        if (!IsWallJumping)
        {
            HandleFlip();
        }

        TryLedgeGrab();
    }

    private void HandleWallSlide()
    {
        if (IsLedgeGrabbing || IsWallJumping)
        {
            ResetWallState();
            return;
        }

        int wallSide = WallSide();
        bool pressingTowardWall = moving_X != 0 && Mathf.Sign(moving_X) == wallSide;

        bool touchingWall = wallSide != 0 && !isGrounded && pressingTowardWall && rigid_bod.linearVelocity.y <= 0f;

        if (!touchingWall)
        {
            ResetWallState();
            return;
        }


        if (WallClingTimer < wallClingTime)
        {
            WallClingTimer += Time.deltaTime;
            IsWallSliding = false;

            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, wallClingFallSpeed);
            return;
        }
        WallSlideTimer += Time.deltaTime;

        if (WallSlideTimer >= wallSlideDelay)
        {
            IsWallSliding = true;
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, Mathf.Clamp(rigid_bod.linearVelocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        else
        {
            IsWallSliding = false;
        }
    }

    private void HandleWallJump()
    {
        if (IsWallSliding)
        {
            IsWallJumping = false;
            WallJumpingDirection = -WallSide();
            WallJumpingCounter = WallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            WallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && WallJumpingCounter > 0f)
        {
            IsWallJumping = true;
            rigid_bod.linearVelocity = new Vector2(WallJumpingDirection * WallJumping_Power.x, WallJumping_Power.y);
            WallJumpingCounter = 0f;
            animator.SetBool("IsJUMPing", true);

            if (transform.localScale.x != WallJumpingDirection)
            {
                Flip();
            }
            Invoke(nameof(StopWallJumping), WallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        IsWallJumping = false;
    }
    private void ResetWallState()
    {
        IsWallSliding = false;
        WallClingTimer = 0f;
        WallSlideTimer = 0f;
    }

    void HandleFlip()
    {
        if (moving_X > 0 && !facingRight)
        {
            Flip();
        }
        else if (moving_X < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator Dash()
    {
        IsWallSliding = false;
        IsWallJumping = false;
        CanDash = false;
        IsDashing = true;
        SpeedMove = baseMoveSpeed;


        float OG_Gravity = rigid_bod.gravityScale;
        rigid_bod.gravityScale = 0f;

        float dashDir;
        Trails.Clear();
        Trails.emitting = true;
        int wallSide = WallSide();

        if (wallSide != 0)
        {
            dashDir = -wallSide;
        }
        else
        {
            dashDir = moving_X != 0 ? Mathf.Sign(moving_X) : Mathf.Sign(transform.localScale.x);
        }

        rigid_bod.linearVelocity = new Vector2(dashDir * Dashing_Power, 1.5f);


        yield return new WaitForSeconds(DashingTime);

        Trails.emitting = false;
        rigid_bod.gravityScale = OG_Gravity;
        IsDashing = false;

        yield return new WaitForSeconds(DashingCooldown);
        CanDash = true;
    }

    private bool CheckGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, playerHalfHeight + 0.1f, LayerMask.GetMask("Ground"));
    }
    private void ApplyBetterGravity()
    {
        if (rigid_bod.linearVelocity.y < 0)
        {
            rigid_bod.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rigid_bod.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rigid_bod.linearVelocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }


    private int WallSide()
    {
        RaycastHit2D right = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, WallLayer);
        RaycastHit2D left = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, WallLayer);

        if (right)
        {
            return 1;
        }
        if (left)
        {
            return -1;
        }
        return 0;
    }

    private bool CheckLedge(int wallSide)
    {
        Vector2 wallCheckPos = (Vector2)transform.position + Vector2.right * wallSide * wallCheckDistance;

        bool wallAtBody = Physics2D.Raycast(transform.position, Vector2.right * wallSide, wallCheckDistance, WallLayer);
        bool wallAtHead = Physics2D.Raycast(transform.position + Vector3.up * ledgeCheckHeight, Vector2.right * wallSide, wallCheckDistance, WallLayer);

        return wallAtBody && !wallAtHead;
    }

    private void TryLedgeGrab()
    {
        if (IsLedgeGrabbing || isGrounded || rigid_bod.linearVelocity.y > 0)
        {
            return;
        }

        int wallSide = WallSide();
        if (wallSide == 0)
        {
            return;
        }

        bool pressingTowardWall = moving_X != 0 && Mathf.Sign(moving_X) == wallSide;

        if (!pressingTowardWall)
        {
            return;
        }

        if (!CheckLedge(wallSide))
        {
            return;
        }

        IsLedgeGrabbing = true;

        originalGravity = rigid_bod.gravityScale;
        rigid_bod.gravityScale = 0f;
        rigid_bod.linearVelocity = Vector2.zero;

        ledgePos = new Vector2(transform.position.x + wallSide * 0.3f, transform.position.y);
        transform.position = ledgePos;

        animator.SetBool("IsLedgeGrabbing", true);
    }

    private void HandleLedgeGrab()
    {
        if (Input.GetButtonDown("Jump"))
        {
            IsLedgeGrabbing = false;
            animator.SetBool("IsLedgeGrabbing", false);

            rigid_bod.gravityScale = originalGravity;

            transform.position += Vector3.up * ledgeClimbUp;
            rigid_bod.linearVelocity = new Vector2(0f, JumpPower);
            return;
        }

        if (moving_X != 0)
        {
            moving_X = 0;
        }

        if (moving_X == 0 && Input.GetAxisRaw("Vertical") < 0)
        {
            ReleaseLedge();
            return;
        }
    }

    private void ReleaseLedge()
    {
        IsLedgeGrabbing = false;
        animator.SetBool("IsLedgeGrabbing", false);
        rigid_bod.gravityScale = originalGravity;
    }

    //public bool canAttack()
    //{
    //    return moving_X == 0 && isGrounded() && !onWall();
    //}
    private void OnDrawGizmosSelected()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir * wallCheckDistance));
        //Gizmos.DrawWireSphere(AttackPoint.transform.position, radius);
    }
}