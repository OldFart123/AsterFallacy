using UnityEngine;

public class Money_Honey : MonoBehaviour, ICollectable
{
    [SerializeField] private AudioClip PickUpSound;

    public int Worth = 1;
    private PersistentID pid;
    private string uniqueID;
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
        //Debug.Log("Money ID: " + uniqueID);
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
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(PickUpSound);
        }

        if (WorldState.Instance != null)
        {
            WorldState.Instance.MarkCollected(uniqueID);
        }

        if (Collectior.instance != null)
        {
            Collectior.instance.IncreaseMoney(Worth);
        }

        Destroy(gameObject);
    }
}