using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;

    [Header("Money UI")]
    [SerializeField] private GameObject moneyCanvas;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private Image backgroundImage;

    [Header("Inventory Menu")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private GameObject loadPanel;

    [Header("Inventory Money Display")]
    [SerializeField] private TMP_Text inventoryMoneyText;
    [SerializeField] private GameObject inventoryMoneyGroup; //parent object that's the icon and text, DO NOT MIX THIS UP AGAIN, ME

    [Header("Key Item Popup")]
    [SerializeField] private Image keyPopupIcon;
    [SerializeField] private float keyPopupDuration = 2f;

    private Coroutine keyPopupRoutine;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameState.IsDialogue)
        {
            PauseGame(!pauseScreen.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !GameState.IsDialogue)
        {
            ToggleInventoryMenu();
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        moneyCanvas.SetActive(false);
        SetAlpha(0f);

        Inventory.Instance.AddItem("Sword");
        Inventory.Instance.AddItem("Potion");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //gameOverScreen = GameObject.Find("GameOverScreen");
        //pauseScreen = GameObject.Find("PauseScreen");

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }

        //Sync money
        if (Collectior.instance != null)
        {
            UpdateMoney(Collectior.instance.CurrentMoney);
        }
    }

    #region Money
    public void UpdateMoney(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = amount.ToString();
        }
    }

    public void ShowMoney()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        moneyCanvas.SetActive(true);

        //Quick fade in
        yield return StartCoroutine(Fade(1f, 1f, 1f));
        //Stay visible
        yield return new WaitForSeconds(5f);
        //Slow fade out
        yield return StartCoroutine(Fade(1f, 0f, 1f));

        moneyCanvas.SetActive(false);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        if (moneyText != null)
        {
            Color c = moneyText.color;
            c.a = alpha;
            moneyText.color = c;
        }

        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = alpha;
            backgroundImage.color = c;
        }
    }
    #endregion Money
    #region GameOver and Pause
    public void GameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }



    public void PauseGame(bool state)
    {
        pauseScreen.SetActive(state);
        Time.timeScale = state ? 0 : 1;
    }
    #endregion GameOver and Pause

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        Time.timeScale = 1;
        GameState.IsPaused = false;
        GameState.IsDialogue = false;
        GameState.IsInventoryOpen = false;

        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Only Editor
#endif
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
    //INVENTORY STUFF!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    #region Inventory
    public void ToggleInventoryMenu()
    {
        bool isActive = inventoryMenu.activeSelf;

        inventoryMenu.SetActive(!isActive);

        if (!isActive)
        {
            ShowInventory();
            RefreshInventoryMoney();

            if (inventoryMoneyGroup != null)
            {
                inventoryMoneyGroup.SetActive(true);
            }
        }
        else
        {
            if (inventoryMoneyGroup != null)
            {
                inventoryMoneyGroup.SetActive(false);
            }
        }

        GameState.IsInventoryOpen = !isActive;
        GameState.IsPaused = !isActive;

        Time.timeScale = isActive ? 1 : 0;
    }
    #region Inventory upper tab buttons
    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
    }

    public void ShowSave()
    {
        inventoryPanel.SetActive(false);
        savePanel.SetActive(true);
        loadPanel.SetActive(false);
    }

    public void ShowLoad()
    {
        inventoryPanel.SetActive(false);
        savePanel.SetActive(false);
        loadPanel.SetActive(true);
    }
    #endregion Intentory upper tab buttons
    #region Inventory actual buttons
    public void SaveGame()
    {
        SaveSystem.SaveGame();
    }

    public void LoadGame()
    {
        SaveSystem.LoadGame();
    }
    #endregion Inventory actual buttons
    public void OpenInventory()
    {
        GameState.IsInventoryOpen = true;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }
    public void CloseInventory()
    {
        GameState.IsInventoryOpen = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
        //if (inventoryMoneyGroup != null)
        //{
        //    inventoryMoneyGroup.SetActive(false);
        //}
    }
    #endregion Inventory
    public void ShowKeyPopup(Sprite icon)
    {
        if (keyPopupIcon == null)
        {
            return;
        }

        keyPopupIcon.sprite = icon;
        keyPopupIcon.gameObject.SetActive(true);

        if (keyPopupRoutine != null)
        {
            StopCoroutine(keyPopupRoutine);
        }

        keyPopupRoutine = StartCoroutine(HideKeyPopup());
    }

    IEnumerator HideKeyPopup()
    {
        yield return new WaitForSeconds(2f);
        keyPopupIcon.gameObject.SetActive(false);
    }

    public void RefreshInventoryMoney()
    {
        if (inventoryMoneyText == null)
        {
            return;
        }

        int money = Collectior.instance != null ? Collectior.instance.CurrentMoney : 0;
        inventoryMoneyText.text = money.ToString();
    }
}