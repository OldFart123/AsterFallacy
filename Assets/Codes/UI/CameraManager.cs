using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    private static List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    public static CinemachineCamera ActiveCamera = null;
    public bool isDefaultCamera = false;

    public static bool CameraLocked = true;

    public static void Register(CinemachineCamera camera)
    {
        if (!cameras.Contains(camera))
        {
            cameras.Add(camera);
        }

        var cm = camera.GetComponent<CameraManager>();
        if (cm != null && cm.isDefaultCamera && ActiveCamera == null)
        {
            ForceDefaultCamera();
        }
    }

    public static void UnRegister(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }

    public static void SwitchCamera(CinemachineCamera newCamera)
    {
        if (CameraLocked || newCamera == null)
        {
            return;
        }

        if (ActiveCamera != null)
        {
            ActiveCamera.Priority = 0;
        }

        newCamera.Priority = 10; // just make it higher than default
        ActiveCamera = newCamera;
    }

    public static void ForceDefaultCamera()
    {
        foreach (var cam in cameras)
        {
            var cm = cam.GetComponent<CameraManager>();
            if (cm != null && cm.isDefaultCamera)
            {
                cam.Priority = 10;
                ActiveCamera = cam;
            }
            else
            {
                cam.Priority = 0;
            }
        }
    }
}