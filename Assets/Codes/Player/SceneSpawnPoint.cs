using UnityEngine;
using System.Collections;

public class SceneSpawnPoint : MonoBehaviour
{
    public string spawnID = "Default";

    private IEnumerator Start()
    {
        yield return null; //frame delay so that player can spawn

        if (SceneTransition.LastSpawnID == spawnID)
        {
            PlayerPersistence player = PlayerPersistence.Instance;

            player.transform.position = transform.position;

            PlayerHealth health = player.GetComponent<PlayerHealth>();

            health.ResetAfterSceneLoad();

            //ForceCameraTrigger(player);//Because Player spawns inside the trigger and makes the camera go to a different camera bound, 2D collider camera, bleeww but doesn't work rn
        }
    }
}