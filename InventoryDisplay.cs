using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public InventoryObjects inventory;
    public int x_start, y_start;
    public int x_space_betweenItems;
    public int numberofColums;
    public int y_space_betweenItems;
    public Dictionary<InventorySlot, GameObject> itemsDislay = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        CreateInventory();
    }

    private void CreateInventory()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var obj = Instantiate(inventory.Container[i].item.prefabs, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");
            itemsDislay.Add(inventory.Container[i], obj);
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_start+(x_space_betweenItems * (i % numberofColums)),y_start+ (-y_space_betweenItems * (i / numberofColums)), 0f);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateDisplayInventory();
    }

    private void UpdateDisplayInventory()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (itemsDislay.ContainsKey(inventory.Container[i]))
            {
                itemsDislay[inventory.Container[i]].GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");

            }
            else
            {
                var obj = Instantiate(inventory.Container[i].item.prefabs, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");
                itemsDislay.Add(inventory.Container[i], obj);

            }
        }
    }
}
