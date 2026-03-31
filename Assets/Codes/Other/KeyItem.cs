using UnityEngine;

public class KeyItem : MonoBehaviour, ICollectable
{
    public string keyID = "CarvedKey";

    private PersistentID pid;
    [SerializeField] private AudioClip PickUpSound;
    [SerializeField] private Sprite keySprite;

    private void Awake()
    {
        pid = GetComponent<PersistentID>();

        if (pid == null)
        {
            pid = gameObject.AddComponent<PersistentID>();
        }
    }

    void Start()
    {
        if (WorldState.Instance.IsCollected(pid.UniqueID))
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
        //Add to inventory
        Inventory.Instance.AddItem(keyID);
        //Collected
        WorldState.Instance.MarkCollected(pid.UniqueID);

        // Play pickup sound
        if (SoundManager.instance != null && PickUpSound != null)
        {
            SoundManager.instance.PlaySound(PickUpSound);
        }

        // Show UI icon
        if (UIManager.Instance != null && keySprite != null)
        {
            UIManager.Instance.ShowKeyPopup(keySprite);
        }

        Destroy(gameObject);
    }
}