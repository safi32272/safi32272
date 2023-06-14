using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    public float missileSpeed = 8f;
    public float accelerateTime = 3f;
    public float Acceleration = 1f;
    public float turnSpeed = 50f;
    public Transform Target;
    public GameObject DestroyParticles, playerDestroyEffect;

    //[HideInInspector]
    public string HitTag;
    [HideInInspector]
    public int DamageTake = 200;

    private bool missileActive = false;
    private bool isAccelerating = false;
    private Rigidbody rb;
    private float accelerateActiveTime = 0;
    private Quaternion guideRotation;
    private bool isTargetTracking = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        CollissionHappened = false;
    }

    public void ActivateMissile ()
    {
        missileActive = true;
        isTargetTracking = true;
        accelerateActiveTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GuideMissile();
    }

    private void Move ()
    {
        //if (Since(accelerateActiveTime) > accelerateTime)
        //    isAccelerating = false;
        //else
        //    isAccelerating = true;

        //if (!missileActive) return;

        //if (isAccelerating)
            missileSpeed += Acceleration * Time.deltaTime;

        rb.velocity = transform.forward * missileSpeed;

        if (isTargetTracking)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, guideRotation, turnSpeed * Time.deltaTime);
    }

    private float Since (float since)
    {
        return Time.time - since;
    }

    private void GuideMissile ()
    {
        if (Target == null) return;

        if (isTargetTracking)
        {
            Vector3 relativePosition = Target.position - transform.position;
            guideRotation = Quaternion.LookRotation(relativePosition, transform.up);
        }
    }

    private bool CollissionHappened = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!CollissionHappened)
        {
            CollissionHappened = true;
            if (collision.transform.tag == HitTag)
            {
                if (collision.transform.GetComponent<Enemy>())
                {
                    Enemy enemy = collision.transform.GetComponent<Enemy>();
                    enemy.TakeHealth(DamageTake);
                    Instantiate(DestroyParticles, transform.position, transform.rotation);
                }
                if (collision.transform.GetComponent<Player>())
                {
                    Player player = collision.transform.GetComponent<Player>();
                    player.DamagePlayer(DamageTake);
                    Instantiate(playerDestroyEffect, transform.position, transform.rotation);
                }   
            }else
            {
                if (collision.transform.tag == "Car")
                {
                    Enemy enemy = collision.transform.GetComponent<Enemy>();
                    enemy.TakeHealth(DamageTake);
                    Instantiate(DestroyParticles, transform.position, transform.rotation);
                }
            }

            Destroy(gameObject);
        }
    }
}
