using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiMang;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiMang = Object.FindFirstObjectByType<UIManager>();
    }

    public void CheckRespawn()
    {
        if(currentCheckpoint == null)
        {
            uiMang.GameOver();
            return;
        }

        playerHealth.Respawn();
        transform.position = currentCheckpoint.position;

        //Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CheckPoint")
        {
            currentCheckpoint = collision.transform;
            SoundManager.instance.PlaySound(checkpoint);
            collision.GetComponent<Collider2D>().enabled = false;
            //collision.GetComponent<Animator>().SetTrigger("Activate");
        }
    }
}
