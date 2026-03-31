using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string path => Application.persistentDataPath + "/save.json";
    public static SaveData pendingLoad = null;

    public static void SaveGame()
    {
        SaveData data = new SaveData();

        //Player
        data.playerHealth = PlayerHealth.Instance.Health;

        //Player Position
        Vector3 pos = PlayerPersistence.Instance.transform.position;

        data.playerPosX = pos.x;
        data.playerPosY = pos.y;

        data.currentScene = SceneManager.GetActiveScene().name;

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
        pendingLoad = JsonUtility.FromJson<SaveData>(json);

        //Set position before scene loads
        PlayerPersistence.LoadedPosition = new Vector2(pendingLoad.playerPosX, pendingLoad.playerPosY);

        SceneManager.LoadScene(pendingLoad.currentScene);
    }
    public static void DeleteSave()
    {
        if (File.Exists(path))// || Input.GetKeyDown(KeyCode.LeftControl))
        {
            File.Delete(path);
            Debug.Log("Save deleted");
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(path);
    }
}