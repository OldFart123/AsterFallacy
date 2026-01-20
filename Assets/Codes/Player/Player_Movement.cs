using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Misc.")]
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private TrailRenderer Trails;
    [SerializeField] private float wallCheckDistance = 0.5f;
    private Rigidbody2D rigid_bod;
    private SpriteRenderer sprite_renderer;
    private Animator animator;
    private BoxCollider2D BoxColli;

    [Header("Movement")]
    public float SpeedMove = 6f;
    public float JumpPower = 7f;
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

    private bool IsWallJumping;
    private float WallJumpingDirection;
    private float WallJumpingTime = 0.3f;
    private float WallJumpingCounter;
    private float WallJumpingDuration = 0.2f;
    public Vector2 WallJumping_Power = new Vector2(2f, 7f);

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
    //SoundManager.instance.PlaySound(JumpSound);

    private void Awake()
    {
        rigid_bod = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        BoxColli = GetComponent<BoxCollider2D>();

        baseMoveSpeed = SpeedMove;
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
    }

    void FixedUpdate()
    {
        if (!isGrounded && jumpMomentum != 0f && !IsWallJumping)
        {
            float accel = jumpMomentum * JumpAcceleration * Time.fixedDeltaTime;
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x + accel, rigid_bod.linearVelocity.y);
        }

        if (IsDashing)
        {
            return;
        }

        if (isGrounded)
        {
            jumpMomentum = 0f;
        }

        if (IsLedgeGrabbing)
        {
            rigid_bod.linearVelocity = new Vector2(0f, rigid_bod.linearVelocity.y);
        }
        else if (!IsWallJumping)
        {
            rigid_bod.linearVelocity = new Vector2(moving_X * SpeedMove, rigid_bod.linearVelocity.y);
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, JumpPower);

            float direction = Mathf.Sign(transform.localScale.x);
            float sprintBonus = IsDashing ? SprintJumpMultiplier : 1f;

            jumpMomentum = JumpForwardForce * sprintBonus * direction;
        }
        //float maxAirSpeed = isSprinting ? SprintSpeed * 1.4f : SpeedMove * 1.2f;

        //rigid_bod.linearVelocity = new Vector2(
        //    Mathf.Clamp(rigid_bod.linearVelocity.x, -maxAirSpeed, maxAirSpeed),
        //    rigid_bod.linearVelocity.y
        //);

        //float direction = moving_X != 0 ? Mathf.Sign(moving_X) : Mathf.Sign(transform.localScale.x);


        if (Input.GetButtonUp("Jump") && rigid_bod.linearVelocity.y > 0)
        {
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, rigid_bod.linearVelocity.y * JumpCutMultiplier);
        }

        if (rigid_bod.linearVelocity.y < 0)
        {
            rigid_bod.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigid_bod.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rigid_bod.linearVelocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
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
                    Trails.emitting = true;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (shiftHeldTimer < SprintHoldTime && CanDash && isGrounded)
            {
                StartCoroutine(Dash());
                Trails.emitting = false;
            }
            ResetSprint();
            Trails.emitting = false;
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
        if (IsLedgeGrabbing)
        {
            IsWallSliding = false;
            return;
        }

        int wallSide = WallSide();

        bool pressingTowardWall = moving_X != 0 && Mathf.Sign(moving_X) == wallSide;

        if (wallSide != 0 && !isGrounded && pressingTowardWall)
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
        CanDash = false;
        IsDashing = true;
        SpeedMove = baseMoveSpeed;

        float OG_Gravity = rigid_bod.gravityScale;
        rigid_bod.gravityScale = 0f;

        rigid_bod.linearVelocity = new Vector2(transform.localScale.x * Dashing_Power, 0f);
        Trails.emitting = true;

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