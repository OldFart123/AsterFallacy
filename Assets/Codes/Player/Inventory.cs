using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public System.Action OnInventoryChanged;

    private List<string> items = new List<string>();
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.parent = null;
    }

    public void AddItem(string id)
    {
        items.Add(id);
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string id)
    {
        return items.Contains(id);
    }

    public void RemoveItem(string id)
    {
        items.Remove(id);
        OnInventoryChanged?.Invoke();
    }

    public List<string> GetAllItems()
    {
        return items;
    }

    public void SetItems(List<string> list)
    {
        items = new List<string>(list);
        OnInventoryChanged?.Invoke();
    }
    public class ItemData : ScriptableObject
    {
        public string id;
        public Sprite icon;
    }
}