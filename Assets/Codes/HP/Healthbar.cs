using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private CharacterHealth playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    void Start()
    {
        if (playerHealth == null && PlayerPersistence.Instance != null)
        {
            playerHealth = PlayerPersistence.Instance.GetComponent<CharacterHealth>();
        }

        totalhealthBar.fillAmount = 0.5f;
    }
    private void LateUpdate()
    {
        if (playerHealth == null && PlayerPersistence.Instance != null)
        {
            playerHealth = PlayerPersistence.Instance.GetComponent<CharacterHealth>();
        }

        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        currenthealthBar.fillAmount = (float)playerHealth.Health / playerHealth.MaxHealth / 2;
    }
}