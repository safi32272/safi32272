﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bullet;
    public bool canAutoFire;
    public float fireRate;
    [HideInInspector]
    public float fireCounter;
    public int currentAmmu;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCounter>0)
        {
            fireCounter-=Time.deltaTime;
        }
    }
}
