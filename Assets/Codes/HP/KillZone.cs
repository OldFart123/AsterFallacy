using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable target = collision.GetComponent<IDamagable>();

        if (target != null)
        {
            target.Damage(target.MaxHealth);
        }
    }
}