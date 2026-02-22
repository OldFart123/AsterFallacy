using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip PickUpSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SoundManager.instance.PlaySound(PickUpSound);
            IDamagable target = collision.GetComponent<IDamagable>();
            if (target != null)
            {
                target.Heal((int)healthValue);
            }
            gameObject.SetActive(false);
        }
    }
}
