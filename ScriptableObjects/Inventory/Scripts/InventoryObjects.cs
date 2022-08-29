using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory",menuName ="Inventory System/Inventory")]
public class InventoryObjects : ScriptableObject    ,ISerializationCallbackReceiver
{
    public ItemDatabaseObject database;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(ItemsObject _item,int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                return;
            }
        }
        Container.Add(new InventorySlot(database.GetId[_item], _item,_amount));
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i <Container.Count ; i++)
        {
            Container[i].item = database.GetItem[Container[i].ID];
        }
    }

    public void OnBeforeSerialize()
    {
     
    }
}
   [System.Serializable]
   public class InventorySlot
{
    public int ID;
    public ItemsObject item;
    public int amount;
    public InventorySlot(int _id,ItemsObject _item,int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}