using UnityEngine;

public class Player_Hit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<IDamagable>() != null)
        {
        collision.gameObject.GetComponent<IDamagable>().Damage(1);
        Debug.Log("Hitting");
        }
    }
}
