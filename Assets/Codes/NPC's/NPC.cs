using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public string npcName;

    [Header("Normal Dialogue")]
    public NPC_Dialogue[] normalDialogues;
    public NPC_Dialogue introDialogue;
    public NPC_Dialogue[] waitingDialogues;
    public NPC_Dialogue completedDialogue;
    public bool allowPostQuestDialogue = false;
    public NPC_Dialogue[] postQuestDialogues;
    private int postQuestIndex = 0;

    [Header("Quest Settings")]
    public bool isQuestNPC = false;
    public string requiredItemID = "CarvedKey";
    public bool disappearOnComplete = true;//For NPC's who like to die (go into shops) or...
    public GameObject rewardPrefab;//...optional reward instead!


    public ShopKeyDoor questDoor;
    private PersistentID pid;

    private int normalIndex = 0;
    private int waitingIndex = 0;

    private QuestStage stage = QuestStage.Normal;

    enum QuestStage
    {
        Normal,
        Intro,
        Waiting,
        Completed
    }

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
        int savedStage = WorldState.Instance.GetNPCState(pid.UniqueID);
        stage = (QuestStage)savedStage;

        if (WorldState.Instance.IsNPCCompleted(pid.UniqueID))
        {
            if (disappearOnComplete)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

    public bool CanInteract()
    {
        return !GameState.IsDialogue;
    }

    public void Interact()
    {
        if (GameState.IsDialogue)
        {
            return;
        }

        if (GameState.GameplayBlocked)
        {
            return;
        }

        if (!isQuestNPC)
        {
            RunNormalNPC();
            return;
        }

        RunQuestNPC();
    }

    void RunNormalNPC()
    {
        if (normalDialogues == null || normalDialogues.Length == 0)
        {
            Debug.LogWarning("NPC has no dialogue!", gameObject);
            return;
        }

        NPC_Dialogue dialogue = normalDialogues[Mathf.Min(normalIndex, normalDialogues.Length - 1)];

        DialogueManager.Instance.StartDialogue(this, dialogue);
    }

    void RunQuestNPC()
    {
        switch (stage)
        {
            case QuestStage.Normal:

                if (normalDialogues != null && normalDialogues.Length > 0)
                {
                    NPC_Dialogue dialogue = normalDialogues[Mathf.Min(normalIndex, normalDialogues.Length - 1)];

                    DialogueManager.Instance.StartDialogue(this, dialogue);
                }
                else if (introDialogue != null)
                {
                    stage = QuestStage.Intro;
                    DialogueManager.Instance.StartDialogue(this, introDialogue);
                }
                else
                {
                    Debug.LogWarning("No Normal or Intro dialogue on " + gameObject.name);
                }

                break;

            case QuestStage.Intro:

                if (introDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(this, introDialogue);
                }
                else
                {
                    Debug.LogWarning("Missing introDialogue on NPC: " + gameObject.name);
                }

                break;

            case QuestStage.Waiting:

                if (Inventory.Instance.HasItem(requiredItemID))
                {
                    if (completedDialogue != null)
                    {
                        DialogueManager.Instance.StartDialogue(this, completedDialogue);
                    }
                    else
                    {
                        Debug.LogWarning("Missing completedDialogue on " + gameObject.name);
                    }
                }
                else
                {
                    if (waitingDialogues != null && waitingDialogues.Length > 0)
                    {
                        int safeIndex = Mathf.Clamp(waitingIndex, 0, waitingDialogues.Length - 1);
                        DialogueManager.Instance.StartDialogue(this, waitingDialogues[safeIndex]);
                    }
                    else
                    {
                        Debug.LogWarning("No waiting dialogues set on " + gameObject.name);
                    }
                }

                break;

            case QuestStage.Completed:

                if (allowPostQuestDialogue && postQuestDialogues != null && postQuestDialogues.Length > 0)
                {
                    NPC_Dialogue dialogue =
                        postQuestDialogues[Mathf.Min(postQuestIndex, postQuestDialogues.Length - 1)];

                    DialogueManager.Instance.StartDialogue(this, dialogue);

                    if (postQuestIndex < postQuestDialogues.Length - 1)
                    {
                        postQuestIndex++;
                    }
                }
                else if (completedDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(this, completedDialogue);
                }
                else
                {
                    Debug.LogWarning("No completed dialogue on " + gameObject.name);
                }

                break;
        }
    }

    public void OnDialogueFinished()
    {
        if (!isQuestNPC)
        {
            if (normalIndex < normalDialogues.Length - 1)
            {
                normalIndex++;
            }

            return;
        }

        switch (stage)
        {
            case QuestStage.Normal:

                if (normalIndex < normalDialogues.Length - 1)
                {
                    normalIndex++;
                }
                else
                {
                    stage = QuestStage.Intro;
                    WorldState.Instance.SetNPCState(pid.UniqueID, (int)stage);
                }

                break;

            case QuestStage.Intro:

                stage = QuestStage.Waiting;
                WorldState.Instance.SetNPCState(pid.UniqueID, (int)stage);
                break;

            case QuestStage.Waiting:

                if (Inventory.Instance.HasItem(requiredItemID))
                {
                    Inventory.Instance.RemoveItem(requiredItemID);

                    stage = QuestStage.Completed;

                    //Save State here
                    WorldState.Instance.SetNPCState(pid.UniqueID, (int)stage);
                    WorldState.Instance.MarkNPCCompleted(pid.UniqueID);

                    if (questDoor != null)
                    {
                        questDoor.UnlockDoor();
                    }

                    if (rewardPrefab != null)
                    {
                        Instantiate(rewardPrefab, transform.position, Quaternion.identity);
                    }

                    if (disappearOnComplete)
                    {
                        gameObject.SetActive(false);
                    }
                }

                break;
        }
    }
}