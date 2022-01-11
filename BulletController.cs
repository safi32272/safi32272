using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed,lifeTime;
    public Rigidbody theRb;
    public GameObject lazerImpact;
    public int demage=1;
    public bool demageEnemey, demagePlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        theRb.velocity=transform.forward*moveSpeed;
        //theRb.AddForce(transform.forward*moveSpeed);
        lifeTime-=Time.deltaTime;
        if (lifeTime<=0)
        {
            Destroy(this.gameObject);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && demageEnemey)
        {
            //Destroy(other.gameObject);
            other.gameObject.GetComponent<EnemyHealthController>().DemageEnemy(demage);
            print(other.tag);
        }
        if (other.gameObject.CompareTag("HeadShot") && demageEnemey)
        {
            other.gameObject.transform.parent.GetComponent<EnemyHealthController>().DemageEnemy(demage*2);
            print("headshot");
        }
        if(other.gameObject.tag=="Player" &&demagePlayer)
        {
            
            PlayerHealthController.instance.DemagePlayer(demage);
        }
        Destroy(this.gameObject);
        Instantiate(lazerImpact, transform.position, transform.rotation);
    }
}
