using UnityEngine;
public interface IDamagable
{
    void Damage(int dmg);
    void Damage(int dmg, Transform attacker);
    int Health { get; }
    int MaxHealth { get; }
    void Heal(int amount);
}