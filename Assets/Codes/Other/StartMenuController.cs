using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject loadButton;
    [SerializeField] private GameObject deleteButton;

    void Start()
    {
        bool hasSave = SaveSystem.SaveExists();

        startButton.SetActive(!hasSave);
        loadButton.SetActive(hasSave);
        deleteButton.SetActive(hasSave);

        RefreshMenu();
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnExitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void OnLoadClick()
    {
        SaveSystem.LoadGame();//fixed, yayy :)
    }

    public void OnDeleteClick()
    {
        SaveSystem.DeleteSave();
        RefreshMenu();
    }
    private void RefreshMenu()
    {
        bool hasSave = SaveSystem.SaveExists();

        startButton.SetActive(!hasSave);
        loadButton.SetActive(hasSave);
        deleteButton.SetActive(hasSave);
    }
}