using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnablesPanels : MonoBehaviour
{
    public GameObject[] panels_Disables;
    public GameObject unlock_panel;
    void Start()
    {
        foreach (var item in panels_Disables)
        {
            item.SetActive(false);
        }
        unlock_panel.SetActive(true);
    }
    public void OnCrossInApp()
    {
        foreach (var item in panels_Disables)
        {
            item.SetActive(true);
        }
        unlock_panel.SetActive(false);
    }

   
}
