using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment Object",menuName ="Inventory System/Items/Equipment")]
public class EquipmentObject : ItemsObject
{
    public int attackbouns;
    public int defencebounse;
    private void Awake()
    {
        itemsType = ItemsType.Equipment;
    }
}
