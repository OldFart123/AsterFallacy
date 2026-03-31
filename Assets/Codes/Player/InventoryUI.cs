using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private TMPro.TMP_Text moneyText;

    public void Refresh()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in Inventory.Instance.GetAllItems())
        {
            GameObject slot = Instantiate(slotPrefab, content);

            Image icon = slot.transform.GetChild(0).GetComponent<Image>();

            icon.sprite = itemDatabase.GetSprite(item);
            icon.enabled = icon.sprite != null;
        }
        RefreshMoneyCounter();
    }
    public void RefreshMoneyCounter()
    {
        if (moneyText == null)
        {
            return;
        }
        int money = Collectior.instance != null ? Collectior.instance.CurrentMoney : 0;

        moneyText.text = money.ToString();
        moneyText.gameObject.SetActive(true); //show counter only in inventory
    }
    public void HideMoneyCounter()
    {
        if (moneyText != null)
        {
            moneyText.gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        Inventory.Instance.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        Inventory.Instance.OnInventoryChanged -= Refresh;
    }
}