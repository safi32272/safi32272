using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObjects inventroy;


    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            inventroy.AddItem(item.item, 1);
        }
    }

    private void OnApplicationQuit()
    {
        inventroy.Container.Clear();
    }
}
