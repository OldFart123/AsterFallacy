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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        moneyCanvas.SetActive(false);
        SetAlpha(0f);
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pauseScreen.activeSelf);
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
}