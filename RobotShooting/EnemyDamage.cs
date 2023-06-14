using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float Health = 200f;
    public GameObject DeadObject;
    public GameObject DestroyParticles;
    public bool DestroyOnDead = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeHealth(float damage)
    {
        if (Health > damage)
        {
            Health -= damage;
        }
        else
        {
            if (DeadObject && !DestroyOnDead)
                Instantiate(DeadObject, transform.position, transform.rotation);
            if (DestroyParticles)
                Instantiate(DestroyParticles, transform.position, transform.rotation);

            if (DestroyOnDead)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
