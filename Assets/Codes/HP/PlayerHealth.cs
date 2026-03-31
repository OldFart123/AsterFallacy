using UnityEngine;
using System.Collections;

public class PlayerHealth : CharacterHealth
{
    public static PlayerHealth Instance;

    [Header("Components")]
    private Animator anim;
    private Player_Movement movement;
    private Rigidbody2D rb;
    private PlayerCombat playerCombat;

    [Header("Knockback")]
    [SerializeField] private float knockbackForceX = 6f;
    [SerializeField] private float knockbackForceY = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;

    [Header("SFX")]
    [SerializeField] private AudioClip hurtSFX;
    [SerializeField] private AudioClip deathSFX;
    protected override void Awake()
    {
        Instance = this;
        base.Awake();

        //LOAD saved health if exists
        if (WorldState.Instance != null && WorldState.Instance.playerHealth > 0)
        {
            Health = WorldState.Instance.playerHealth;
        }
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<Player_Movement>();
        rb = GetComponent<Rigidbody2D>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    public override void Damage(int dmg)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        base.Damage(dmg);

        if (WorldState.Instance != null)
        {
            WorldState.Instance.playerHealth = Health;
        }
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);

        if (WorldState.Instance != null)
        {
            WorldState.Instance.playerHealth = Health;
        }
    }

    protected override void OnHurt()
    {
        if (Health <= 0)
        {
            return;
        }

        anim.SetTrigger("Hurt");

        if (playerCombat != null)
        {
            playerCombat.ForceResetAttack();
        }

        SoundManager.instance.PlaySound(hurtSFX);

        if (lastAttacker != null)
        {
            StartCoroutine(KnockbackRoutine(lastAttacker));
        }

        StartCoroutine(HitPause(0.2f));
    }

    protected override void Die()
    {
        anim.SetTrigger("Die");
        SoundManager.instance.PlaySound(deathSFX);

        if (playerCombat != null)
        {
            playerCombat.ForceResetAttack();
        }

        movement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<PlayerRespawn>().CheckRespawn();
    }
    public void ResetAfterSceneLoad()
    {
        if (WorldState.Instance != null && WorldState.Instance.playerHealth > 0)
        {
            Health = WorldState.Instance.playerHealth;
        }
        else
        {
            Health = MaxHealth;
        }

        isInvincible = false;

        if (movement != null)
        {
            movement.enabled = true;
            movement.StopAutoWalk();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.Play("Idle");
        }
    }
    public override void Respawn()
    {
        Health = MaxHealth;

        if (WorldState.Instance != null)
        {
            WorldState.Instance.playerHealth = Health;
        }

        isInvincible = true;

        anim.ResetTrigger("Die");
        anim.Play("Idle");

        movement.enabled = true;

        StartCoroutine(RespawnInvulnerability());
    }

    private IEnumerator RespawnInvulnerability()
    {
        yield return new WaitForSeconds(1f);
        isInvincible = false;
    }

    private IEnumerator KnockbackRoutine(Transform attacker)
    {
        movement.canMove = false;

        float direction = transform.position.x < attacker.position.x ? -1f : 1f;

        rb.linearVelocity = new Vector2(direction * knockbackForceX, knockbackForceY);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        movement.canMove = true;
    }
}