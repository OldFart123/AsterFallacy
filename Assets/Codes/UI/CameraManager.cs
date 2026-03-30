using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    static List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    public static CinemachineCamera ActiveCamera = null;
    public bool isDefaultCamera = false;

    public static bool IsActiveCamera(CinemachineCamera camera)
    {
        return camera == ActiveCamera;
    }
    public static void SwitchCamera(CinemachineCamera newCamera)
    {
        newCamera.Priority = 10;
        ActiveCamera = newCamera;

        foreach(CinemachineCamera cam in cameras)
        {
            if (cam != newCamera)
            {
                cam.Priority = 0;
            }
        }
    }
    public static void Register(CinemachineCamera camera)
    {
        cameras.Add(camera);

        CameraManager cm = camera.GetComponent<CameraManager>();

        if (cm != null && cm.isDefaultCamera)
        {
            SwitchCamera(camera);
            return;
        }

        if (ActiveCamera == null)
        {
            SwitchCamera(camera);
        }
    }
    //public static void Register(CinemachineCamera camera)
    //{
    //    cameras.Add(camera);

    //    //if (camera.GetComponent<CameraTag>().isDefaultCamera)
    //    //{
    //    //    SwitchCamera(camera);
    //    //    return;
    //    //}

    //    if (ActiveCamera == null)
    //    {
    //        SwitchCamera(camera);
    //    }
    //}
    public static void UnRegister(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var mainCam = GameObject.FindWithTag("MainCamera");

        if (mainCam != null)
        {
            var cine = mainCam.GetComponent<CinemachineCamera>();
            CameraManager.SwitchCamera(cine);
        }
    }
}
