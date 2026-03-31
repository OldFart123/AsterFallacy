using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineReconnect : MonoBehaviour
{
    CinemachineCamera cam;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupCamera());
    }

    IEnumerator SetupCamera()
    {
        CameraManager.CameraLocked = true;
        //Wait until everything finished, which includes the player spawn because the player spawn timing is super important as it sometimes falls off the map if they're loaded in too fast.
        yield return new WaitForEndOfFrame();

        CameraManager.ForceDefaultCamera();

        yield return null; //small delay so priority applies

        if (PlayerPersistence.Instance != null)
        {
            Transform target = PlayerPersistence.Instance.transform;

            cam.Follow = target;
            cam.LookAt = target;

            cam.ForceCameraPosition(target.position, Quaternion.identity);

            cam.PreviousStateIsValid = false;
        }
        //unlocks when everything can WORK, if not I will cry
        yield return new WaitForSeconds(0.1f);
        CameraManager.CameraLocked = false;
    }
}