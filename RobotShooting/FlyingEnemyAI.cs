using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public float MoveSpeed = 8f;
    public float accelerateTime = 3f;
    public float Acceleration = 1f;
    public float turnSpeed = 50f;
    public Transform Player;
    public float ActiveRange = 50f;
    public float StoppingDistance = 10f;
    public float velocityDropRate = 0.9f;

    private bool isActive = false;
    private bool isAccelerating = false;
    private Rigidbody rb;
    private float accelerateActiveTime = 0;
    private Quaternion guideRotation;
    private bool isTargetTracking = false;
    [HideInInspector]
    public bool reachedTarget = false;
    private float distanceToPlayer = 0;

    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Player = FindObjectOfType<Player>().transform;
        ActivateFlight();
    }

    public void ActivateFlight ()
    {
        isActive = true;
        isTargetTracking = true;
        accelerateActiveTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Player != null)
                distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            else if (Player.gameObject.activeSelf == false)
                Player = FindObjectOfType<Player>().transform;
        }
        Movement();
        SetDirection();
    }

    private void Movement()
    {
        if (Since(accelerateActiveTime) > accelerateTime)
            isAccelerating = false;
        else
            isAccelerating = true;

        if (!isActive) return;

        if (isAccelerating)
            MoveSpeed += Acceleration * Time.deltaTime;
        if (distanceToPlayer > StoppingDistance)
        {
            reachedTarget = false;
            rb.velocity = transform.forward * MoveSpeed;
        }else
        {
            reachedTarget = true;
            rb.velocity = rb.velocity * velocityDropRate;
        }

        if (isTargetTracking)
        {
            Quaternion quaternion = Quaternion.RotateTowards(transform.rotation, guideRotation, turnSpeed * Time.deltaTime);
            Vector3 temp = new Vector3(0, quaternion.eulerAngles.y, 0);
            quaternion.eulerAngles = temp;
            transform.rotation = quaternion;
        }
    }

    private float Since (float since)
    {
        return Time.time - since;
    }

    private void SetDirection()
    {
        if (Player == null) return;

        if (isTargetTracking)
        {
            Vector3 relativePosition = Player.position - transform.position;
            guideRotation = Quaternion.LookRotation(relativePosition, transform.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
