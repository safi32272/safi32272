using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    public List<GameObject> gameObjects;
    int current_object;
    public Button click,toolsChnge;
    public GameObject activePlayer;

    void Awake()
    {
        current = this;
        current_object = 0;
        //print(current)
        activePlayer = gameObjects[current_object];
    }
    public void ChangeObject()
    {
        current_object++;
        if (current_object >= gameObjects.Count)
        {
            current_object = 0;
        }
        //foreach (var item in gameObjects)
        //{
        //    item.SetActive(false);


        //}
        //gameObjects[current_object].SetActive(true);
        activePlayer = gameObjects[current_object];
    }
    public static event Action<int> onDoorTriggerEnter;
    public static void OnDoorTriggerEvent(int id)
    {
        //print(id);
        //print(current.gameObjects[current.current_object].transform.GetChild(0).GetComponent<tools>().id);
        if(onDoorTriggerEnter!=null && current.activePlayer.transform.GetChild(0)!=null&& id==current.activePlayer.transform.GetChild(0).GetComponent<tools>().id)
        {
            //current.click.onClick.RemoveAllListeners();
            GameEvents.current.click.onClick.AddListener(() => onDoorTriggerEnter(id));
           
        }
    }
           public static event Action<int> onTriggerExited;
    public static void OnTriggerExiting(int id)
    {
        if (onTriggerExited != null)
        {
           GameEvents.current.click.onClick.AddListener(()=> onTriggerExited(id));
        }
    }

    public static event Action<int> onToolsChanger;
    public static void OnToolChanger(int _id)
    {
        if (onToolsChanger != null && current.activePlayer.transform.GetChild(0) != null && _id == current.activePlayer.transform.GetChild(0).GetComponent<tools>().id)
        {
            current.toolsChnge.onClick.AddListener(() => onToolsChanger(_id));
        }
    }
    
}  
