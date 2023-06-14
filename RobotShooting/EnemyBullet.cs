using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int DamageTake = 10;
    public float AutoDestroyTime = 5f;
    public GameObject ExplosionObject;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, AutoDestroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.root.GetComponent<Player>())
            {
                Player player = collision.transform.root.GetComponent<Player>();
                player.DamagePlayer(DamageTake);
            }
        }
        if (ExplosionObject)
        {
            Instantiate(ExplosionObject, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
