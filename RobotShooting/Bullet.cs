using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isShotgunBullet = false;

    [HideInInspector]
    public float DamageTake = 100f;
    [HideInInspector]
    public string HitTag;
    [HideInInspector]
    public GameObject ExplosionObject;

    public Transform[] Children;

    // Start is called before the first frame update
    void Start()
    {
        
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            if (collision.transform.tag == "Enemy" || collision.transform.tag == "Car")
            {
                if (collision.gameObject.GetComponent<Enemy>())
                {
                    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                    enemy.TakeHealth(DamageTake);
                }
            }
            if (ExplosionObject != null)
            {
                GameObject temp = Instantiate(ExplosionObject, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
