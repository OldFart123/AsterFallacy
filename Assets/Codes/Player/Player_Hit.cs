using Unity.VisualScripting;
using UnityEngine;

public class Player_Hit : MonoBehaviour
{
    [SerializeField] private AudioClip HurtingSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            collision.gameObject.GetComponent<IDamagable>().Damage(1);
            SoundManager.instance.PlaySound(HurtingSound);

            //Debug.Log("Hitting");
        }
    }
}
