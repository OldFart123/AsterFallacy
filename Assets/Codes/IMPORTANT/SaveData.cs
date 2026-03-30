using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int playerHealth;
    public int money;
    public int CarvedKey;

    public List<string> collectedItems = new List<string>();
    public List<string> openedChests = new List<string>();
    public List<string> inventoryItems = new List<string>();
    public List<string> completedNPCs = new List<string>();
    public List<string> unlockedDoors = new List<string>();
    public List<string> npcIDs = new List<string>();
    public List<int> npcStages = new List<int>();
}