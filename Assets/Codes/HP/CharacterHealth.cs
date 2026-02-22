using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour, IDamagable
{
    [Header("Health")]
    [SerializeField] protected int maxHealth = 10;
    public int MaxHealth => maxHealth;
    public int Health { get; protected set; }
    protected Transform lastAttacker;

    [Header("IFrames")]
    [SerializeField] protected float invincibleTime = 0.5f;
    protected bool isInvincible;

    protected virtual void Awake()
    {
        Health = maxHealth;
    }

    // Standard damage
    public virtual void Damage(int dmg)
    {
        if (isInvincible || Health <= 0) return;

        Health -= dmg;

        if (Health <= 0)
            Die();
        else
        {
            StartCoroutine(Invincibility());
            OnHurt();
        }
    }

    // Damage with attacker reference
    public virtual void Damage(int dmg, Transform attacker)
    {
        lastAttacker = attacker;
        Damage(dmg);
    }

    protected virtual void OnHurt() { }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }

    protected IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    protected IEnumerator HitPause(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    public virtual void Respawn()
    {
        Health = MaxHealth;
        isInvincible = false;
    }
}
