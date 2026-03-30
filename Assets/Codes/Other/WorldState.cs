using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static WorldState Instance;
    public int playerHealth = -1;
    public int playerMaxHealth = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //transform.SetParent(null);//newest change

        //DontDestroyOnLoad(gameObject);
    }

    private HashSet<string> collectedObjects = new HashSet<string>();
    private HashSet<string> openedChests = new HashSet<string>();
    private Dictionary<string, int> npcStates = new Dictionary<string, int>();
    private HashSet<string> completedNPCs = new HashSet<string>();


    public bool IsCollected(string id) => collectedObjects.Contains(id);
    public void MarkCollected(string id) => collectedObjects.Add(id);

    public bool IsChestOpened(string id) => openedChests.Contains(id);
    public void MarkChestOpened(string id) => openedChests.Add(id);

    public HashSet<string> GetCollected() => collectedObjects;
    public HashSet<string> GetOpenedChests() => openedChests;

    public Dictionary<string, int> GetAllNPCStates() => npcStates;
    public bool IsNPCCompleted(string id) => completedNPCs.Contains(id);
    public void MarkNPCCompleted(string id) => completedNPCs.Add(id);

    private HashSet<string> unlockedDoors = new HashSet<string>();

    public bool IsDoorUnlocked(string id) => unlockedDoors.Contains(id);

    public void UnlockDoor(string id) => unlockedDoors.Add(id);

    public HashSet<string> GetUnlockedDoors() => unlockedDoors;

    public void SetUnlockedDoors(List<string> list)
    {
        unlockedDoors = new HashSet<string>(list);
    }

    public void SetCollected(List<string> list)
    {
        collectedObjects = new HashSet<string>(list);
    }

    public void SetOpenedChests(List<string> list)
    {
        openedChests = new HashSet<string>(list);
    }
    public int GetNPCState(string id)
    {
        if (npcStates.ContainsKey(id))
        {
            return npcStates[id];
        }

        return 0; //Default = Normal
    }

    public void SetNPCState(string id, int state)
    {
        npcStates[id] = state;
    }

    public void SetAllNPCStates(Dictionary<string, int> states)
    {
        npcStates = states;
    }

    public HashSet<string> GetCompletedNPCs() => completedNPCs;

    public void SetCompletedNPCs(List<string> list)
    {
        completedNPCs = new HashSet<string>(list);
    }
}