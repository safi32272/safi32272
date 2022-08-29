using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemsType
{
    Default,
    Food,
    Equipment
}
public abstract class ItemsObject : ScriptableObject
{
    public GameObject prefabs;
    public ItemsType itemsType;

    [TextArea(15,20)]
    public string Discriptio;
}
