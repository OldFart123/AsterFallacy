using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private RectTransform panel;
    [SerializeField] private float transitionTime = 0.5f;

    private bool isTransitioning = false;
    private bool shouldReveal = false;
    private Vector2 lastDirection;
    public static string LastSpawnID;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        panel.gameObject.SetActive(false);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Refind this image every scene
        if (panel == null)
        {
            Debug.LogError("TransitionPanel not found in scene!");
            return;
        }

        if (shouldReveal)
        {
            StartCoroutine(RevealRoutine());
        }
        else
        {
            panel.gameObject.SetActive(false);
            isTransitioning = false;
        }
    }

    public void Transition(int sceneIndex, Vector2 direction, string spawnID)
    {
        if (isTransitioning)
        {
            return;
        }

        lastDirection = direction;
        LastSpawnID = spawnID;

        StartCoroutine(TransitionRoutine(sceneIndex));
    }

    private IEnumerator TransitionRoutine(int sceneIndex)
    {
        isTransitioning = true;
        shouldReveal = true;

        if (panel == null)
        {
            Debug.LogError("Panel missing, forcing scene load");
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            yield break;
        }

        panel.gameObject.SetActive(true);

        Vector2 startPos = lastDirection * Screen.width;
        Vector2 endPos = Vector2.zero;

        panel.anchoredPosition = startPos;

        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, timer / transitionTime);
            yield return null;
        }

        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }

    private IEnumerator RevealRoutine()
    {
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = -lastDirection * Screen.width;

        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, timer / transitionTime);
            yield return null;
        }

        panel.gameObject.SetActive(false);
        shouldReveal = false;
        isTransitioning = false;
    }
}