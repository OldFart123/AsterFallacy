using UnityEngine;

public class HealthCollectible : MonoBehaviour, ICollectable
{
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip PickUpSound;

    private string uniqueID;
    private PersistentID pid;

    void Awake()
    {
        pid = GetComponent<PersistentID>();
        if (pid == null)
        {
            pid = gameObject.AddComponent<PersistentID>();
        }
    }

    void Start()
    {
        uniqueID = pid.UniqueID;

        if (WorldState.Instance != null && WorldState.Instance.IsCollected(uniqueID))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
    public void Collect()
    {
        SoundManager.instance.PlaySound(PickUpSound);

        if (PlayerPersistence.Instance == null)
        {
            return;
        }

        IDamagable player = PlayerPersistence.Instance.GetComponent<IDamagable>();
        if (player == null)
        {
            return;
        }

        player.Heal((int)healthValue);

        if (WorldState.Instance != null)
        {
            WorldState.Instance.MarkCollected(uniqueID);
        }

        Destroy(gameObject);
    }
}