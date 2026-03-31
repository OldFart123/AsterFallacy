using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera targetCamera;

    // Track if this trigger currently controls the camera
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || CameraManager.CameraLocked)
        {
            return;
        }

        // Switch to this trigger camera
        CameraManager.SwitchCamera(targetCamera);
        isActive = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || CameraManager.CameraLocked)
        {
            return;
        }

        // Only restore default if this trigger was active
        if (isActive)
        {
            CameraManager.ForceDefaultCamera();
            isActive = false;
        }
    }
}