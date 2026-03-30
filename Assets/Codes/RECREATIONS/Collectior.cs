using UnityEngine;

public class Collectior : MonoBehaviour
{
    public static Collectior instance;

    public int CurrentMoney = 0;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void IncreaseMoney(int value)
    {
        CurrentMoney += value;

        UIManager.Instance?.UpdateMoney(CurrentMoney);
        UIManager.Instance?.ShowMoney(); //trigger fade
    }
}