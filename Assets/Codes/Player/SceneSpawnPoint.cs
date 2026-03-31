using UnityEngine;
using System.Collections;

public class SceneSpawnPoint : MonoBehaviour
{
    public string spawnID = "Default";

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame(); //frame delay so that player can spawn
        //yield return null;
        if (SceneTransition.LastSpawnID == spawnID)
        {
            PlayerPersistence player = PlayerPersistence.Instance;

            player.transform.position = transform.position;

            PlayerHealth health = player.GetComponent<PlayerHealth>();

            health.ResetAfterSceneLoad();

        }
    }
}