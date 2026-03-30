using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;
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
        }
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        movement.StopAutoWalk();
        movement.enabled = true;
    }
}