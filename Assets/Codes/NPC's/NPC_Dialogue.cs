using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC_Dialogue", menuName = "NPC Talks")]
public class NPC_Dialogue : ScriptableObject
{
    public string npcName;

    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public Sprite portrait;
    }

    public DialogueLine[] lines;

    public float typingSpeed = 0.05f;

    public AudioClip talkSound;

    [System.Serializable]
    public class DialogueChoice
    {
        public string text;
        public NPC_Dialogue nextDialogue;
    }

    public DialogueChoice[] choices;
}