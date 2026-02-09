using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Collectior : MonoBehaviour
{
    public static Collectior instance;

    public GameObject moneyCanvas;
    public TMP_Text moneyText;
    public Image backgroundImage;

    public int CurrentMoney = 0;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        moneyCanvas.SetActive(false);
        SetAlpha(0f);
        moneyText.text = CurrentMoney.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectable Item = collision.GetComponent<ICollectable>();
        if (Item != null) //&& collision.tag == "Player")
        {
            Item.Collect();
            //Debug.Log("Collected");
        }
        //if (collision.gameObject.GetComponent<ICollectable>() != null)
        //{
        //    collision.gameObject.GetComponent<ICollectable>().Damage(1);
        //    Debug.Log("Collected");
        //}
    }

    public void IncreaseMoney(int value)
    {
        CurrentMoney += value;
        moneyText.text = CurrentMoney.ToString();
        ShowCounter();
    }

    void ShowCounter()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        moneyCanvas.SetActive(true);

        //Quick fade in
        yield return StartCoroutine(Fade(1f, 1f, 1f));
        //Stay visible
        yield return new WaitForSeconds(5f);
        //Slow fade out
        yield return StartCoroutine(Fade(1f, 0f, 1f));

        moneyCanvas.SetActive(false);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        Color textColor = moneyText.color;
        textColor.a = alpha;
        moneyText.color = textColor;

        Color bgColor = backgroundImage.color;
        bgColor.a = alpha;
        backgroundImage.color = bgColor;
    }
}
