using UnityEngine;
using System.Collections;

public class PlayerHealth : CharacterHealth
{
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

    private void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<Player_Movement>();
        rb = GetComponent<Rigidbody2D>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    protected override void OnHurt()
    {
        if (Health <= 0)
        {
            return;
        }

        anim.SetTrigger("Hurt");
        SoundManager.instance.PlaySound(hurtSFX);

        if (lastAttacker != null)
        {
            StartCoroutine(KnockbackRoutine(lastAttacker));
        }

        playerCombat.cannotAttack = false;
        StartCoroutine(HitPause(0.2f));
    }

    protected override void Die()
    {
        anim.SetTrigger("Die");
        SoundManager.instance.PlaySound(deathSFX);

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

    public override void Respawn()
    {
        Health = MaxHealth;
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
        playerCombat.cannotAttack = false;
        float direction = transform.position.x < attacker.position.x ? -1f : 1f;

        rb.linearVelocity = new Vector2(direction * knockbackForceX, knockbackForceY);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        movement.canMove = true;
        playerCombat.cannotAttack = true;
    }
}
