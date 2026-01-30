using UnityEngine;

public class Clones : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.15f;
    [SerializeField] private float fadeSpeed = 8f;

    private SpriteRenderer sr;
    private Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
        Destroy(gameObject, lifeTime);
    }

    public void Init(Sprite sprite, Vector3 scale, Color tint)
    {
        sr.sprite = sprite;
        transform.localScale = scale;
        color = tint;
        sr.color = color;
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;
    }
}