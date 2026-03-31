using UnityEngine;
using System.Collections;

public class EnemyHealth : CharacterHealth
{
    [Header("SFX")]
    [SerializeField] private AudioClip hurtSFX;
    [SerializeField] private AudioClip deathSFX;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("Death Settings")]

    private bool isDying = false;

    private void Update()
    {
        if (GameState.GameplayBlocked)
        {
            return;
        }
    }
    protected override void OnHurt()
    {
        if (isDying)
        {
            return;
        }

        if (anim != null) anim.SetTrigger("Hurt");
        SoundManager.instance.PlaySound(hurtSFX);

        StartCoroutine(HitPause(0.3f));
    }
    public override void Damage(int dmg)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (isDying)
        {
            return;
        }

        base.Damage(dmg);
    }
    protected override void Die()
    {
        if (isDying) return;
        {
            isDying = true;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        EnemyPatrol patrol = GetComponentInParent<EnemyPatrol>();
        if (patrol != null)
        {
            patrol.enabled = false;
        }
        transform.SetParent(null);
        if (patrol != null)
        {
            Destroy(patrol.gameObject);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            //rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 2f;
            rb.constraints = RigidbodyConstraints2D.None;
        }

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        SoundManager.instance.PlaySound(deathSFX);
        StartCoroutine(DeathRoutine());
    }
    public void ApplyKnockback(Vector2 force)
    {
        StartCoroutine(KnockbackRoutine(force));
    }

    private IEnumerator KnockbackRoutine(Vector2 force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        EnemyPatrol patrol = GetComponentInParent<EnemyPatrol>();

        if (patrol != null)
        {
            patrol.enabled = false;
        }

        //if (rb != null)
        //{
        //    rb.linearVelocity = force;
        //}

        yield return new WaitForSeconds(0.2f);

        if (patrol != null)
        {
            patrol.enabled = true;
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
