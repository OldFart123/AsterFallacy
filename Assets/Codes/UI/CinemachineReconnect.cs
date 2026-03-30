using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CinemachineReconnect : MonoBehaviour
{
    CinemachineCamera cam;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        SceneManager.sceneLoaded += Reconnect;
    }

    void Reconnect(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPersistence.Instance != null)
        {
            cam.Follow = PlayerPersistence.Instance.transform;
            cam.LookAt = PlayerPersistence.Instance.transform;
        }
    }
}