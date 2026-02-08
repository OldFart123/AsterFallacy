using UnityEngine;

public class Money_Honey : MonoBehaviour, ICollectable
{
    [SerializeField] private AudioClip PickUpSound;
    public int Worth = 1;
    public void Collect()
    {
        SoundManager.instance.PlaySound(PickUpSound);
        Destroy(gameObject);
        Collectior.instance.IncreaseMoney(Worth);
    }
}