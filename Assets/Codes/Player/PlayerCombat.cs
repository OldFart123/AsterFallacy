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
    private bool isHurt;

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
        if (movement == null)
        {
            movement = GetComponent<Player_Movement>();
        }
        hitbox = attackHitbox.GetComponent<Player_Hit>();
        hitboxCollider = attackHitbox.GetComponent<BoxCollider2D>();
        attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (GameState.GameplayBlocked)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            comboStep = 0;
        }
        //If movement is disabled mid attack, cancel attack and force reset
        if (isAttacking && CannotStartAttack())
        {
            ForceResetAttack();
        }
    }
    private bool CannotStartAttack()
    {
        if (movement == null)
        {
            return true;
        }

        if (!movement.canMove)
        {
            return true;
        }

        if (movement.IsCurrentlyDashing)
        {
            return true;
        }

        if (movement.IsCurrentlyLedgeGrabbing)
        {
            return true;
        }

        if (movement.IsCurrentlyWallClinging)
        {
            return true;
        }

        if (movement.IsCurrentlyWallSliding)
        {
            return true;
        }

        return false;
    }

    private void TryAttack()
    {
        if (isAttacking)
        {
            attackQueued = true;
            {
                return;
            }
        }
        if (CannotStartAttack())
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        bool grounded = movement.IsGroundedPublicated;
        float vertical = Input.GetAxisRaw("Vertical");

        int attackType = 0;

        if (grounded)
        {
            if (vertical > 0.5f)
            {
                attackType = 1; //Ground Up
                comboStep = 0;
            }
            else
            {
                attackType = 0; //Ground combo
                comboStep++;
                if (comboStep > maxCombo)
                {
                    comboStep = 1;
                }
            }
        }
        else
        {
            comboStep = 0;

            if (vertical > 0.5f)
            {
                attackType = 3; //Air Up
            }
            else if (vertical < -0.5f)
            {
                attackType = 4; //Air Down
            }
            else
            {
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
        isAttacking = false;

        if (attackQueued)
        {
            attackQueued = false;
            StartAttack();
        }
    }
    public void ForceResetAttack()
    {
        isAttacking = false;
        attackQueued = false;
        comboStep = 0;
        comboTimer = 0f;

        attackHitbox.SetActive(false);

        animator.ResetTrigger("Attack");
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
        //Debug.Log("Hitbox Offset: " + hitboxCollider.offset);

        //Resize collider for shit hoooolllyy I'm tired
        //hitboxCollider.size = data.size;

        //Flip offset based on the player's current facing direction
        //Vector2 offset = data.offset;

        //Check if the player is facing right or left based on the player's localScale.x
        //float facingDirection = Mathf.Sign(transform.localScale.x);
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //Multiply the offset by the facing direction
        //offset.x *= facingDirection;

        //Apply the offset to the hitbox collider
        //hitboxCollider.offset = offset;
        hitboxCollider.offset = data.offset;

        //Pass attack values to Player_Hit
        hitbox.SetAttackValues(data.damage, data.knockbackX, data.knockbackY, facingDirection);

        attackHitbox.SetActive(true);
    }
}