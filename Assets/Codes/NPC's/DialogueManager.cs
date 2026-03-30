using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portrait;

    private NPC currentNPC;
    private NPC_Dialogue currentDialogue;

    private int index;
    private bool isTyping;
    private bool blockInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!GameState.IsDialogue)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && !blockInput)
        {
            NextLine();
        }
    }

    public void StartDialogue(NPC npc, NPC_Dialogue dialogue)
    {
        if (GameState.IsDialogue)
        {
            return;
        }

        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
        {
            Debug.LogError("Tried to start NULL or EMPTY dialogue!", npc);
            return;
        }

        currentNPC = npc;
        currentDialogue = dialogue;

        GameState.IsDialogue = true;
        Time.timeScale = 0;

        index = 0;

        nameText.text = dialogue.npcName;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
        StartCoroutine(BlockInputForFrame());
    }

    IEnumerator TypeLine()
    {
        if (currentDialogue == null || currentDialogue.lines.Length == 0)
        {
            Debug.LogError("Dialogue is invalid");
            EndDialogue();
            yield break;
        }

        isTyping = true;

        var line = currentDialogue.lines[index];

        if (line == null)
        {
            Debug.LogError("Dialogue line is null >:(");
            EndDialogue();
            yield break;
        }

        if (portrait != null)
        {
            portrait.sprite = line.portrait;
            portrait.enabled = line.portrait != null;
        }

        dialogueText.text = "";

        //yield return null;

        foreach (char letter in line.text)
        {
            dialogueText.text += letter;

            if (currentDialogue.talkSound != null)
            {
                SoundManager.instance?.PlaySound(currentDialogue.talkSound);
            }

            yield return new WaitForSecondsRealtime(currentDialogue.typingSpeed);
        }

        isTyping = false;
    }
    void NextLine()
    {
        if (isTyping)
        {
            //skipping typing to show full textbox line
            StopAllCoroutines();
            dialogueText.text = currentDialogue.lines[index].text;
            isTyping = false;
            return;
        }

        index++;

        if (index < currentDialogue.lines.Length)
        {
            //if There's another line, it'll type the next one.
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }
    public void EndDialogue()
    {
        StopAllCoroutines();

        GameState.IsDialogue = false;
        Time.timeScale = 1;

        //CameraManager.SwitchCamera(playerCamera);
        dialoguePanel.SetActive(false);

        currentNPC?.OnDialogueFinished();

        currentDialogue = null;
    }

    IEnumerator BlockInputForFrame()
    {
        blockInput = true;
        yield return null; //Wait for 1 frame so that the first dialog line doesn't instantly appear all at once
        blockInput = false;
    }
}