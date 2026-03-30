using UnityEngine;
using System.Collections.Generic;

public class Player_Hit : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip hurtingSound;

    [Header("Default Knockback (can be overridden)")]
    [SerializeField] private float enemyKnockbackX = 6f;
    [SerializeField] private float enemyKnockbackY = 3f;

    private float currentDamage = 1f;
    private float currentKnockbackX;
    private float currentKnockbackY;
    private float facingDirection = 1f;

    private HashSet<IDamagable> hitTargets = new HashSet<IDamagable>();

    private void Awake()
    {
        currentKnockbackX = enemyKnockbackX;
        currentKnockbackY = enemyKnockbackY;
    }

    private void OnEnable()
    {
        hitTargets.Clear();
    }

    //Called by PlayerCombat before enabling the hitbox.
    public void SetAttackValues(float damage, float knockbackX, float knockbackY, float direction)
    {
        currentDamage = damage;
        currentKnockbackX = knockbackX;
        currentKnockbackY = knockbackY;
        facingDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable target = collision.GetComponent<IDamagable>();

        if (target == null)
        {
            return;
        }

        if (target.Health <= 0)
        {
            return;
        }

        if (hitTargets.Contains(target))
        {
            return;
        }

        hitTargets.Add(target);

        //Apply damage
        target.Damage((int)currentDamage, transform.root);

        //Pogo, air kick down to cancel damage
        Player_Movement movement = GetComponentInParent<Player_Movement>();
        Rigidbody2D playerRB = movement.GetComponent<Rigidbody2D>();

        Collider2D playerCol = movement.GetComponent<Collider2D>();
        Collider2D enemyCol = collision.GetComponent<Collider2D>();

        bool falling = playerRB.linearVelocity.y < 0f;

        //Player bottom must be above enemy top (tee-hee)
        bool aboveEnemy = playerCol.bounds.min.y >= enemyCol.bounds.max.y - 0.05f;

        //Player must overlap horizontally with enemy center (prevents a side pogo so it's less stupid)
        bool horizontallyCentered = playerCol.bounds.center.x > enemyCol.bounds.min.x && playerCol.bounds.center.x < enemyCol.bounds.max.x;

        if (falling && aboveEnemy && horizontallyCentered)
        {
            movement.PogoBounce();
        }

        //Apply knockback if enemy has EnemyHealth, but doesn't work most of the time because it need the enemy to have gravity so we just roll lol
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Vector2 knockbackForce = new Vector2(facingDirection * currentKnockbackX, currentKnockbackY);

            enemy.ApplyKnockback(knockbackForce);
        }

        //Play hit sound
        if (hurtingSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(hurtingSound);
        }
    }
}