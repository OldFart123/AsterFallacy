using UnityEngine;
using System.Collections;

public class Chests : MonoBehaviour, IInteractable
{
    [Header("Loot")]
    public GameObject[] itemPrefabs;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private float openDelay = 0.4f; //Match animation timing

    [Header("Pop-Up Settings")]
    [SerializeField] private float upForce = 6f;
    [SerializeField] private float sideForce = 1.5f;

    private bool isOpened;
    private PersistentID pid;

    private void Awake()
    {
        pid = GetComponent<PersistentID>();
        if (pid == null)
        {
            pid = gameObject.AddComponent<PersistentID>();
        }
    }

    private void Start()
    {
        if (WorldState.Instance.IsChestOpened(pid.UniqueID))
        {
            isOpened = true;

            if (animator != null)
            {
                animator.SetBool("IsOpened", true);
            }
        }
    }

    public bool CanInteract() => !isOpened;

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        isOpened = true;
        WorldState.Instance.MarkChestOpened(pid.UniqueID);

        if (animator != null)
        {
            animator.SetTrigger(openTrigger);
            animator.SetBool("IsOpened", true);
        }

        StartCoroutine(SpawnLootAfterDelay());
    }

    private IEnumerator SpawnLootAfterDelay()
    {
        yield return new WaitForSeconds(openDelay);

        foreach (GameObject item in itemPrefabs)
        {
            if (item == null)
            {
                continue;
            }

            GameObject spawned = Instantiate(item, transform.position, Quaternion.identity);

            Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float randomX = Random.Range(-sideForce, sideForce);
                float randomY = Random.Range(upForce * 0.8f, upForce * 1.2f);

                rb.linearVelocity = new Vector2(randomX, randomY);
            }
        }
    }
}