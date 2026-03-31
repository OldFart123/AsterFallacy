using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;
    public static Vector2? LoadedPosition = null;
    //private bool justSpawned = true;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        //justSpawned = false;
    }

    void Awake()
    {
        //Debug.Log("PlayerPersistence exists: " + Instance);
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (GameRoot.Instance != null)
        {
            transform.SetParent(GameRoot.Instance.transform);
        }
        else
        {
            Debug.LogWarning("GameRoot missing!");
        }
        //DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var movement = GetComponent<Player_Movement>();

        if (movement != null)
        {
            movement.StopAutoWalk();
            movement.enabled = true;
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        //Save data gere
        if (SaveSystem.pendingLoad != null)
        {
            var data = SaveSystem.pendingLoad;

            WorldState.Instance.playerHealth = data.playerHealth;
            Collectior.instance.CurrentMoney = data.money;

            WorldState.Instance.SetCollected(data.collectedItems);
            WorldState.Instance.SetOpenedChests(data.openedChests);

            Inventory.Instance.SetItems(data.inventoryItems);

            //NPC states
            Dictionary<string, int> npcStates = new Dictionary<string, int>();
            for (int i = 0; i < data.npcIDs.Count; i++)
            {
                npcStates[data.npcIDs[i]] = data.npcStages[i];
            }

            WorldState.Instance.SetAllNPCStates(npcStates);
            WorldState.Instance.SetCompletedNPCs(data.completedNPCs);

            WorldState.Instance.SetUnlockedDoors(data.unlockedDoors);

            //Clear pending load
            SaveSystem.pendingLoad = null;
        }

        UIManager.Instance.UpdateMoney(Collectior.instance.CurrentMoney);

        if (LoadedPosition.HasValue)
        {
            transform.position = LoadedPosition.Value;
            LoadedPosition = null;
        }
    }
}