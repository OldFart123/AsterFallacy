using UnityEngine;
using System.Collections;

public class ShopKeyDoor : MonoBehaviour, IInteractable
{
    [Header("Scene Transition")]
    public int sceneBuildIndex;
    public Vector2 enterDirection;
    public string targetSpawnID;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private float openDelay = 0.4f;

    private bool isOpened;
    private bool isAnimating;
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
        if (WorldState.Instance.IsDoorUnlocked(pid.UniqueID))
        {
            isOpened = true;

            if (animator != null)
            {
                animator.SetBool("IsOpened", true);
            }
        }
    }

    public bool CanInteract()
    {
        return !GameState.GameplayBlocked && !isAnimating && isOpened;
    }

    public void Interact()
    {
        if (!isOpened || isAnimating)
        {
            return;
        }

        Enter();
    }

    //called by the NPC
    public void UnlockDoor()
    {
        if (isOpened)
        {
            return;
        }

        isOpened = true;
        isAnimating = true;

        WorldState.Instance.UnlockDoor(pid.UniqueID);

        if (animator != null)
        {
            animator.SetTrigger(openTrigger);
            animator.SetBool("IsOpened", true);
        }

        StartCoroutine(FinishOpening());
    }

    private IEnumerator FinishOpening()
    {
        yield return new WaitForSeconds(openDelay);
        isAnimating = false;
    }

    private void Enter()
    {
        Player_Movement player = PlayerPersistence.Instance.GetComponent<Player_Movement>();
        player.StartAutoWalk(enterDirection, 0.3f);

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.Transition(sceneBuildIndex, enterDirection, targetSpawnID);
        }
    }
}