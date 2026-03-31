using UnityEngine;

[DisallowMultipleComponent]
public class PersistentID : MonoBehaviour
{
    [SerializeField] private string uniqueID;

    public string UniqueID => uniqueID;
}