using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private HashSet<string> items = new HashSet<string>();

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
    }

    public bool HasItem(string id)
    {
        return items.Contains(id);
    }

    public void RemoveItem(string id)
    {
        items.Remove(id);
    }

    public List<string> GetAllItems()
    {
        return new List<string>(items);
    }

    public void SetItems(List<string> list)
    {
        items = new HashSet<string>(list);
    }
}