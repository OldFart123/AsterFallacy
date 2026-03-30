using UnityEngine;

public class LevelMoving : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 enterDirection; //set in inspector (1,0) right, (-1,0) left, (0,1) up, (0,-1) down.
    public string targetSpawnID;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D entity)
    {
        if (hasTriggered)
        {
            return;
        }

        if (!entity.CompareTag("Player"))
        {
            return;
        }

        hasTriggered = true;

        Player_Movement player = entity.GetComponent<Player_Movement>();
        player.StartAutoWalk(enterDirection, 0.5f);

        if (SceneTransition.Instance != null)
        {
            //Debug.Log("Transition triggered");
            SceneTransition.Instance.Transition(sceneBuildIndex, enterDirection, targetSpawnID);
        }
    }
}
//    private void OnTriggerEnter2D(Collider2D entity)
//    {
//        if (!entity.CompareTag("Player"))
//        {
//            return;
//        }

//        Player_Movement player = entity.GetComponent<Player_Movement>();

//        player.StartAutoWalk(enterDirection, 0.5f);

//        if (SceneTransition.Instance != null)
//        {
//            SceneTransition.Instance.Transition(sceneBuildIndex, enterDirection, targetSpawnID);
//        }
//        //debugging to fix huge bugger
//        if (SceneTransition.Instance != null)
//        {
//            Debug.Log("Transition triggered");
//            SceneTransition.Instance.Transition(sceneBuildIndex, enterDirection, targetSpawnID);
//        }
//        else
//        {
//            Debug.LogError("SceneTransition is NULL!");
//        }
//    }
//}