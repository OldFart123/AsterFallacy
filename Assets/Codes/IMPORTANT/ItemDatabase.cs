using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public string id;
        public Sprite icon;
    }

    public List<ItemData> items;

    public Sprite GetSprite(string id)
    {
        foreach (var item in items)
        {
            if (item.id == id)
            {
                return item.icon;
            }
        }
        return null;
    }
}