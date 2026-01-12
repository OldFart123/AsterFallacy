using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    private float StartingPosition;
    private float length;
    public GameObject Cams;
    public float ParallaxEffect;

    void Start()
    {
        StartingPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distannce = Cams.transform.position.x * ParallaxEffect;
        float movement = Cams.transform.position.x * (1 - ParallaxEffect);
        
        transform.position = new Vector3(StartingPosition + distannce, transform.position.y, transform.position.z);
        if(movement > StartingPosition + length)
        {
            StartingPosition += length;
        }
        else if(movement < StartingPosition - length)
        {
            StartingPosition -= length;
        }
    }
}
