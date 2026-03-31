using System.Collections;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    #region Headers
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
    public bool canMove = true;

    private bool isAutoWalking;
    private float autoWalkTimer;
    private Vector2 autoWalkDirection;

    private PlayerSlopeHandler slopeHandler;

    [Header("Dashing")]
    public float Dashing_Power = 10f;
    public float DashingTime = 0.2f;
    public float DashingCooldown = 1f;
    private bool CanDash = true;
    private bool IsDashing;
    public bool IsCurrentlyDashing => IsDashing;

    [Header("Sprinting")]
    public float SprintSpeed = 10f;
    public float SprintHoldTime = 0.2f;
    private float shiftHeldTimer;
    private float baseMoveSpeed;
    private bool isSprinting;

    [Header("WallSliding")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float wallSlideDelay = 0.08f;
    private bool IsWallSliding;
    private float WallSlidingSpeed = 3f;
    private float WallSlideTimer;
    public bool IsCurrentlyWallSliding => IsWallSliding;

    [Header("WallCling")]
    [SerializeField] private float wallClingTime = 0.3f;
    [SerializeField] private float wallClingFallSpeed = 0f;
    private float WallClingTimer;
    private bool IsWallClinging;
    public bool IsCurrentlyWallClinging => IsWallClinging;

    [Header("WallJumping")]
    private float WallJumpingDirection;
    private float WallJumpingTime = 0.3f;
    private float WallJumpingCounter;
    private float WallJumpingDuration = 0.2f;
    private bool IsWallJumping;
    public Vector2 WallJumping_Power = new Vector2(1.5f, 7f);

    [Header("Jump Height")]
    [Range(0.1f, 1f)]
    public float JumpCutMultiplier = 0.6f;

    [Header("Jump Momentum")]
    public float SprintJumpMultiplier = 1.15f;
    private float jumpTakeoffSpeed;

    [Header("Gravity Control")]
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    private bool facingRight = true;

    private bool isGrounded;
    #region Mess up fix in code
    public float VerticalVelocity
    {
        get
        {
            return rigid_bod.linearVelocity.y;
        }
    }

    public void DyingHorz()
    {
        rigid_bod.linearVelocity = new Vector2(0, 0);
    }

    public bool IsGroundedPublic
    {
        get
        {
            return isGrounded;
        }
    }
    public bool IsGroundedPublicated => isGrounded;
    public float VerticalSpeed => rigid_bod.linearVelocity.y;
    public float HorizontalSpeed => Mathf.Abs(rigid_bod.linearVelocity.x);
    #endregion

    private float playerHalfHeight;

    [Header("Ledge Grab")]
    [SerializeField] private float ledgeCheckHeight = 0.4f;
    [SerializeField] private float ledgeClimbUp = 0.6f;
    public bool IsCurrentlyLedgeGrabbing => IsLedgeGrabbing;

    [Header("Air Control")]
    [SerializeField] private float airAcceleration = 7f;
    [SerializeField] private float airMaxSpeed;

    private bool IsLedgeGrabbing;
    private Vector2 ledgePos;
    private float originalGravity;

    [Header("Pogo Bounce")]
    [SerializeField] private float pogoBounceForce = 5f;

    [Header("Sprint Afterimages")]
    [SerializeField] private Clones afterimagePrefab;
    [SerializeField] private float afterimageSpawnRate = 0.05f;
    [SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.6f);

    private float afterimageTimer;

    [Header("SFX")]
    private IPlayerSFX sfx;

    #endregion Headers

    #region The Basic Three Codes
    private void Awake()
    {
        rigid_bod = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        BoxColli = GetComponent<BoxCollider2D>();
        sfx = GetComponent<IPlayerSFX>();
        slopeHandler = GetComponent<PlayerSlopeHandler>();

        baseMoveSpeed = SpeedMove;
        airMaxSpeed = SprintSpeed * 1.1f;
        playerHalfHeight = sprite_renderer.bounds.extents.y;
    }

    void Update()
    {
        if (GameState.GameplayBlocked || !canMove)
        {
            rigid_bod.linearVelocity = new Vector2(0, rigid_bod.linearVelocity.y);
            return;
        }

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

        HandleJumpInput();
        HandleDashOrSprint();
        HandleMovement();
        HandleWallSlide();
        HandleWallJump();
        HandleSprintAfterimages();

        UpdateAnimator();
    }


    void FixedUpdate()
    {
        if (!canMove || IsDashing || IsLedgeGrabbing)
        {
            return;
        }

        if (isAutoWalking)
        {
            rigid_bod.linearVelocity = new Vector2(autoWalkDirection.x * SpeedMove, rigid_bod.linearVelocity.y);

            autoWalkTimer -= Time.fixedDeltaTime;

            if (autoWalkTimer <= 0f)
            {
                isAutoWalking = false;
                canMove = true;
            }

            return;
        }

        float targetSpeed = moving_X * SpeedMove;

        if (isGrounded)
        {
            jumpTakeoffSpeed = rigid_bod.linearVelocity.x;

            if (slopeHandler.IsOnSlope)
            {
                slopeHandler.HandleSlopeMovement(targetSpeed, true);
            }
            else
            {
                rigid_bod.linearVelocity = new Vector2(targetSpeed, rigid_bod.linearVelocity.y);
            }
        }
        else
        {
            float speedDiff = targetSpeed - rigid_bod.linearVelocity.x;
            float accel = airAcceleration * Time.fixedDeltaTime;
            float movement = Mathf.Clamp(speedDiff, -accel, accel);

            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x + movement, rigid_bod.linearVelocity.y);
        }
        ApplyBetterGravity();

        if (isGrounded)
        {
            ResetWallState();
        }
    }

    #endregion The Basic Three Codes

    #region Updated Animation code
    private void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rigid_bod.linearVelocity.y);
        animator.SetFloat("IsRunning", Mathf.Abs(rigid_bod.linearVelocity.x));
        animator.SetBool("IsSprinting", isSprinting && Mathf.Abs(rigid_bod.linearVelocity.x) > 0.1f);
        animator.SetBool("IsWallSliding", IsWallSliding);
        animator.SetBool("IsWallClinging", IsWallClinging);
    }
    #endregion Updated Animation code

    #region Jump, HandleMovement, Pogo and BetterGravity
    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            sfx?.PrJumping();
            jumpTakeoffSpeed = rigid_bod.linearVelocity.x;
            float sprintBoost = isSprinting ? SprintJumpMultiplier : 1f;
            rigid_bod.linearVelocity = new Vector2(jumpTakeoffSpeed, JumpPower * sprintBoost);
        }

        if (Input.GetButtonUp("Jump") && rigid_bod.linearVelocity.y > 0)
        {
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, rigid_bod.linearVelocity.y * JumpCutMultiplier);
        }
    }

    private void HandleMovement()
    {
        if (!IsWallJumping)
        {
            HandleFlip();
        }

        TryLedgeGrab();
    }
    public void PogoBounce()
    {
        float bounce = pogoBounceForce;

        if (Input.GetButton("Jump"))
        {
            bounce *= 1.1f; //Stronger bounce if holding jump
        }

        rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, bounce);
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
    #endregion Jump, HandleMovement, Pogo and BetterGravity

    #region Dash and Sprint
    private void HandleDashOrSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !IsDashing)
        {
            shiftHeldTimer += Time.deltaTime;

            if (shiftHeldTimer >= SprintHoldTime)
            {
                SpeedMove = SprintSpeed;
                isSprinting = true;
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
    private IEnumerator Dash()
    {
        sfx?.PrDashing();
        IsWallSliding = false;
        IsWallJumping = false;
        CanDash = false;
        IsDashing = true;
        SpeedMove = baseMoveSpeed;
        moving_X = 0f;


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

        rigid_bod.linearVelocity = new Vector2(dashDir * Dashing_Power, 1.1f);


        yield return new WaitForSeconds(DashingTime);

        Trails.emitting = false;
        rigid_bod.gravityScale = OG_Gravity;
        IsDashing = false;

        yield return new WaitForSeconds(DashingCooldown);
        CanDash = true;
    }
    private void ResetSprint()
    {
        shiftHeldTimer = 0f;
        SpeedMove = baseMoveSpeed;
        isSprinting = false;
    }
    #endregion Dash and Sprint

    #region WallSlide
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
            sfx?.PrStartWallSlide();
            rigid_bod.linearVelocity = new Vector2(rigid_bod.linearVelocity.x, Mathf.Clamp(rigid_bod.linearVelocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        else
        {
            IsWallSliding = false;
            sfx?.PrStopWallSlide();
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
    #endregion WallSlide

    #region WallJumping and reset WallState
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
            animator.SetBool("IsGrounded", false);

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
        sfx?.PrStopWallSlide();
        IsWallSliding = false;
        WallClingTimer = 0f;
        WallSlideTimer = 0f;
    }
    #endregion WallJumping and reset WallState

    #region Flipping
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
    #endregion Flipping

    #region LedgeGrab
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
    #endregion LedgeGrab
    #region After Images and AutoWalk
    private void HandleSprintAfterimages()
    {
        if (!isSprinting || Mathf.Abs(rigid_bod.linearVelocity.x) < 0.2f)
        {
            afterimageTimer = 0f;
            return;
        }

        afterimageTimer -= Time.deltaTime;

        if (afterimageTimer <= 0f)
        {
            SpawnAfterimage();
            afterimageTimer = afterimageSpawnRate;
        }
    }
    private void SpawnAfterimage()
    {
        Clones img = Instantiate(afterimagePrefab, transform.position, Quaternion.identity);

        SpriteRenderer playerSprite = sprite_renderer;

        img.Init(playerSprite.sprite, transform.localScale, afterimageColor);
    }

    public void StartAutoWalk(Vector2 direction, float duration)
    {
        isAutoWalking = true;
        canMove = false;
        autoWalkDirection = direction.normalized;
        autoWalkTimer = duration;
    }

    public void AutoWalkTo(Vector3 target, float speed)
    {
        StartCoroutine(AutoWalkRoutine(target, speed));
    }

    IEnumerator AutoWalkRoutine(Vector3 target, float speed)
    {
        canMove = false;

        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.unscaledDeltaTime);
            yield return null;
        }

        canMove = true;
    }
    public void StopAutoWalk()
    {
        isAutoWalking = false;
        canMove = true;
    }
    #endregion After Images and AutoWalk

    #region Raycasts
    private bool CheckGrounded()
    {
        Bounds bounds = BoxColli.bounds;

        Vector2 leftOrigin = new Vector2(bounds.min.x + 0.05f, bounds.min.y);
        Vector2 rightOrigin = new Vector2(bounds.max.x - 0.05f, bounds.min.y);

        float rayLength = 0.3f;

        bool leftGround = Physics2D.Raycast(leftOrigin, Vector2.down, rayLength, GroundLayer);
        bool rightGround = Physics2D.Raycast(rightOrigin, Vector2.down, rayLength, GroundLayer);

        return leftGround || rightGround;
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir * wallCheckDistance));
        //Gizmos.DrawWireSphere(AttackPoint.transform.position, radius);
    }
    #endregion Raycasts
}