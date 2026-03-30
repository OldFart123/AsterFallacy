using UnityEngine;

public class KeyItem : MonoBehaviour, ICollectable
{
    public string keyID = "CarvedKey";

    private PersistentID pid;

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
        Inventory.Instance.AddItem(keyID);

        WorldState.Instance.MarkCollected(pid.UniqueID);

        Destroy(gameObject);
    }
}