using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object Database",menuName ="Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject ,ISerializationCallbackReceiver
{
    public ItemsObject[] item;
    public Dictionary<ItemsObject, int> GetId = new Dictionary<ItemsObject, int>();
    public Dictionary<int, ItemsObject> GetItem = new Dictionary<int, ItemsObject>();

    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<ItemsObject, int>();
        GetItem = new Dictionary<int, ItemsObject>();
        for (int i = 0; i < item.Length; i++)
        {
            GetId.Add(item[i], i);
            GetItem.Add(i, item[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
}
