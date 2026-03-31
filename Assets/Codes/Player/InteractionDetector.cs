using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; //Closest Interactable
    public GameObject InteractionIcon;

    void Start()
    {
        InteractionIcon.SetActive(false);
    }

    void Update()
    {
        if (GameState.GameplayBlocked)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactableInRange != null && interactableInRange.CanInteract()) //Works if it's an NPC or chest or whatever!
            {
                interactableInRange.Interact();
            }
        }
    }
    public void ForceHideIcon()
    {
        InteractionIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract()) //If it's interactable and if so...
        {
            interactableInRange = interactable;
            InteractionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange) //Moved away so it's not interactable
        {
            interactableInRange = null;
            InteractionIcon.SetActive(false);
        }
    }
}