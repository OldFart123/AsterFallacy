using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlatformEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class PassThroughPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private Collider2D col;

    private void Awake()
    {
        effector = GetComponent<PlatformEffector2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(DisableCollisionTemporarily());
        }
    }

    private IEnumerator DisableCollisionTemporarily()
    {
        //Disable the collider so player can fall through
        col.enabled = false;

        //Wait a short time so player falls
        yield return new WaitForSeconds(0.60f);

        //Re-enable collider after that!
        col.enabled = true;
    }
}