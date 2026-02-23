using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Player_Movement movement;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private int maxCombo = 2;

    private int comboStep = 0;
    private float comboTimer;
    private bool isAttacking;
    private bool attackQueued;
    public bool cannotAttack = false;

    private Player_Hit hitbox;
    private BoxCollider2D hitboxCollider;

    [System.Serializable]
    public class AttackData
    {
        public Vector2 size;
        public Vector2 offset;
        public float damage;
        public float knockbackX;
        public float knockbackY;
    }

    [Header("Attack Data")]
    [SerializeField] private AttackData kickData;
    [SerializeField] private AttackData tailBashData;
    //[SerializeField] private AttackData lastComboData; //not finished yet, too lazy to make another animation for the 3 way combo attack rn
    [SerializeField] private AttackData groundUpData;
    [SerializeField] private AttackData airUpData;
    [SerializeField] private AttackData airDownData;
    [SerializeField] private AttackData airForwardData;

    private void Awake()
    {
        if(movement == null)
        {
        movement = GetComponent<Player_Movement>();
        }
        hitbox = attackHitbox.GetComponent<Player_Hit>();
        hitboxCollider = attackHitbox.GetComponent<BoxCollider2D>();
        attackHitbox.SetActive(false);
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mousebutton down");
            TryAttack();
        }

        if (comboTimer > 0)
        {
            Debug.Log("combo timer");
            comboTimer -= Time.deltaTime;
        }
        else
        {
            Debug.Log("combo be 0 if anything");
            comboStep = 0;
        }
    }
    private bool CannotStartAttack()
    {
        if (movement == null)
        {
            Debug.Log("movement bye bye");
            return true;
        }

        if (!movement.canMove)
        {
            Debug.Log("cannot attack if unable to attack");
            return true;
        }

        if (movement.IsCurrentlyDashing)
        {
            Debug.Log("cannot attack if dashing");
            return true;
        }

        if (movement.IsCurrentlyLedgeGrabbing)
        {
            Debug.Log("cannot attack if ledge grabbing");
            return true;
        }

        if (movement.IsCurrentlyWallClinging)
        {
            Debug.Log("cannot attack if wall clinging");
            return true;
        }

        if (movement.IsCurrentlyWallSliding)
        {
            Debug.Log("cannot attack if wall sliding");
            return true;
        }
        if (!cannotAttack)
        {
            Debug.Log("cannot attack if hurt :)");
            return true;
        }
        Debug.Log("cannot START attack");
        return false;
    }

    private void TryAttack()
    {
        if (isAttacking)
        {
            Debug.Log("if isattackuing");
            attackQueued = true;
            {
                Debug.Log("returned attack que");
                return;
            }
        }
        if (CannotStartAttack())
        {
            Debug.Log("returned cannot start attack");
            return;
        }
        Debug.Log("trying to attack");
        StartAttack();
    }

    private void StartAttack()
    {
        Debug.Log("Start attack");
        bool grounded = movement.IsGroundedPublicated;
        float vertical = Input.GetAxisRaw("Vertical");

        int attackType = 0;

        if (grounded)
        {
            Debug.Log("grounded shit");
            if (vertical > 0.5f)
            {
                Debug.Log("ground up");
                attackType = 1; //Ground Up
                comboStep = 0;
            }
            else
            {
                Debug.Log("Ground combo");
                attackType = 0; //Ground combo
                comboStep++;
                if (comboStep > maxCombo)
                {
                    Debug.Log("if over max combo");
                    comboStep = 1;
                }
            }
        }
        else
        {
            Debug.Log("else for air");
            comboStep = 0;

            if (vertical > 0.5f)
            {
                Debug.Log("air up");
                attackType = 3; //Air Up
            }
            else if (vertical < -0.5f)
            {
                Debug.Log("air down");
                attackType = 4; //Air Down
            }
            else
            {
                Debug.Log("air forward");
                attackType = 2; //Air Forward
            }
        }

        animator.SetInteger("AttackType", attackType);
        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");

        comboTimer = comboResetTime;
        isAttacking = true;
    }


    //Mostly just animation even nonsense this plays at the last frame of an attack unless you want everything to break sighhh
    public void EndAttack()
    {
        Debug.Log("End attack");
        isAttacking = false;

        if (attackQueued)
        {
            Debug.Log("End attack Que");
            attackQueued = false;
            StartAttack();
        }
    }
    #region Animation Event shit
    public void EnableKick()
    {
        ActivateHitbox(kickData);
    }

    public void EnableTailBash()
    {
        ActivateHitbox(tailBashData);
    }
    //public void EnableLastCombo()
    //{
    //    ActivateHitbox(lastComboData);
    //}
    public void EnableGroundUp()
    {
        ActivateHitbox(groundUpData);
    }
    public void EnableAirUp()
    {
        ActivateHitbox(airUpData);
    }

    public void EnableAirDown()
    {
        ActivateHitbox(airDownData);
    }
    public void EnableAirForward()
    {
        ActivateHitbox(airForwardData);
    }
    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }
    #endregion Animation Event shit
    #region gizmo see
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (kickData != null)
        {
            DrawAttackGizmo(kickData, Color.green);
        }

        if (tailBashData != null)
        {
            DrawAttackGizmo(tailBashData, Color.blue);
        }

        //if (lastComboData != null)
        //{
        //    DrawAttackGizmo(lastComboData, Color.black);
        //}

        if (groundUpData != null)
        {
            DrawAttackGizmo(groundUpData, Color.brown);
        }

        if (airUpData != null)
        {
            DrawAttackGizmo(airUpData, Color.yellow);
        }

        if (airForwardData != null)
        {
            DrawAttackGizmo(airForwardData, Color.purple);
        }

        if (airDownData != null)
        {
            DrawAttackGizmo(airDownData, Color.red);
        }
    }

    private void DrawAttackGizmo(AttackData data, Color color)
    {
        Gizmos.color = color;

        Vector2 facingOffset = data.offset;

        float facingDirection = 1f;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            facingDirection = 1f;
        }
        else
        {
            facingDirection = Mathf.Sign(transform.localScale.x);
        }
#endif

        facingOffset.x *= facingDirection;

        Vector3 center = transform.position + (Vector3)facingOffset;

        Gizmos.DrawWireCube(center, data.size);
    }
#endif
    #endregion gizmo see
    
    private void ActivateHitbox(AttackData data)
    {
        Debug.Log("Activate HitBox");
        //Resize collider for shit hoooolllyy I'm tired
        hitboxCollider.size = data.size;

        //Flip offset based on the player's current facing direction
        Vector2 offset = data.offset;

        //Check if the player is facing right or left based on the player's localScale.x
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //Multiply the offset by the facing direction
        offset.x *= facingDirection;

        //Apply the offset to the hitbox collider
        hitboxCollider.offset = offset;

        //Pass attack values to Player_Hit
        hitbox.SetAttackValues(data.damage, data.knockbackX, data.knockbackY, facingDirection);

        attackHitbox.SetActive(true);
    }
}
