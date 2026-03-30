using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string path => Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData data = new SaveData();

        //Player
        data.playerHealth = PlayerHealth.Instance.Health;

        //Money
        data.money = Collectior.instance.CurrentMoney;

        //World state
        data.collectedItems = new List<string>(WorldState.Instance.GetCollected());
        data.openedChests = new List<string>(WorldState.Instance.GetOpenedChests());

        //Inventory
        data.inventoryItems = new List<string>(Inventory.Instance.GetAllItems());

        //NPCs
        foreach (var pair in WorldState.Instance.GetAllNPCStates())
        {
            data.npcIDs.Add(pair.Key);
            data.npcStages.Add(pair.Value);
        }

        data.completedNPCs = new List<string>(WorldState.Instance.GetCompletedNPCs());

        //Door
        data.unlockedDoors = new List<string>(WorldState.Instance.GetUnlockedDoors());

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Game Saved");
    }

    public static void LoadGame()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save found");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //Player
        WorldState.Instance.playerHealth = data.playerHealth;

        //Money
        Collectior.instance.CurrentMoney = data.money;

        //World
        WorldState.Instance.SetCollected(data.collectedItems);
        WorldState.Instance.SetOpenedChests(data.openedChests);

        //Inventory
        Inventory.Instance.SetItems(data.inventoryItems);

        //NPC states
        Dictionary<string, int> npcStates = new Dictionary<string, int>();

        for (int i = 0; i < data.npcIDs.Count; i++)
        {
            npcStates[data.npcIDs[i]] = data.npcStages[i];
        }

        WorldState.Instance.SetAllNPCStates(npcStates);
        WorldState.Instance.SetCompletedNPCs(data.completedNPCs);

        //Door
        WorldState.Instance.SetUnlockedDoors(data.unlockedDoors);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Game Loaded");
    }

    public static void DeleteSave()
    {
        if (File.Exists(path) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            File.Delete(path);
            Debug.Log("Save deleted");
        }
    }
}