using UnityEngine;

public class LootBox : MonoBehaviour, IDamagable
{
    //get damage
    public int Health { get; set; }
    public int maxHealth = 10;

    public void Start()
    {
        //start health to be maxhealth
        Health = maxHealth;
    }
    public void Damage(int dmg)
    {
        //current health
        Debug.Log($"{gameObject} received {dmg} damage");
        Health -= dmg;
        //dead
        if(Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
