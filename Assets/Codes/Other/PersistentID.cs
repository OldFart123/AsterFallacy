using UnityEngine;

[DisallowMultipleComponent]
public class PersistentID : MonoBehaviour
{
    [SerializeField] private string uniqueID;

    public string UniqueID => uniqueID;
}
//using UnityEngine;
//using System;

//[DisallowMultipleComponent]
//public class PersistentID : MonoBehaviour
//{
//    [SerializeField, HideInInspector] private string uniqueID;

//    public string UniqueID => uniqueID;

//    //void Start()
//    //{
//    //    Debug.Log(gameObject.name + " ID: " + uniqueID);
//    //}

//    private void Awake()
//    {
//        //if (string.IsNullOrEmpty(uniqueID))
//        //{
//        //    uniqueID = Guid.NewGuid().ToString();
//        //}
//        //This made a new ID everytime the scene loaded, even when leaving and returning to the scene, no matter if it was picked up, an infinite collectable glitch so to say
//    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        if (string.IsNullOrEmpty(uniqueID))
//        {
//            uniqueID = Guid.NewGuid().ToString();
//            UnityEditor.EditorUtility.SetDirty(this);
//        }
//    }
//#endif
//}