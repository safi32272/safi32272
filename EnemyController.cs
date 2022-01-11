using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    //public float moveSpeed;
    //public Rigidbody theRb;
    public bool chase;
    public float distanceToChase=10f, distanceToLose = 15f,distanceToStop=2f;
    public Vector3 targetPoint;
    public NavMeshAgent agent;

    Vector3 startPos;

    public float keepChasingTimer=5f;
    private float chaseCounter;

    public GameObject bullet;
    public Transform bulletPoint;
    public float fireRate,waitBetweenShoot=2f,timeToShoot=1f;
    private float fireCount,shotWaitCounter,shootTimecounter;
    public Animator anim;

    void Start()
    {
        startPos=this.transform.position;
        shootTimecounter=timeToShoot;
        shotWaitCounter=waitBetweenShoot;
        print(shootTimecounter);
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint=PlayerController.instance.transform.position;
        targetPoint.y=transform.position.y;
        if (!chase)
        {
            if (Vector3.Distance(transform.position, targetPoint)<distanceToChase)
            {
                chase=true;
                shootTimecounter=timeToShoot;
                shotWaitCounter=waitBetweenShoot;
            }
            if (chaseCounter>0)
            {
            chaseCounter-=Time.deltaTime;
            if (chaseCounter<=0)
            {
                agent.destination=startPos;
            }
            }
            //print(agent.remainingDistance);
            if (agent.remainingDistance<0.25f)
            {

                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            //transform.LookAt(targetPoint); //look toward the target point
            //theRb.velocity=transform.forward*moveSpeed;   //the object move toward the target point
            if (Vector3.Distance(transform.position, targetPoint)>distanceToStop)
            {
                agent.destination=targetPoint;
            }
            else
            {
                agent.destination=transform.position;
                agent.transform.LookAt(targetPoint);
            }
           

            if (Vector3.Distance(transform.position, targetPoint)>distanceToLose)
            {
                chase=false;
                //agent.destination=startPos;
                chaseCounter=keepChasingTimer;
            }
           
            if (shotWaitCounter>0)
            {
                shotWaitCounter-=Time.deltaTime;
               
                if (shotWaitCounter<=0)
                {
                    shootTimecounter=timeToShoot; 
                }
                anim.SetBool("isMoving", true);
            }
            else
            {
                if (PlayerController.instance.gameObject.activeInHierarchy)
                {

              
            shootTimecounter-=Time.deltaTime;
            if (shootTimecounter>0)
            {

                fireCount-=Time.deltaTime;
                if (fireCount<=0)
                {
                    fireCount=fireRate;
                        bulletPoint.LookAt(PlayerController.instance.transform.position+new Vector3(0f,1.2f,0f));

                        Vector3 targetDir = PlayerController.instance.transform.position-transform.position;
                        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
                        if (Mathf.Abs(angle)<30)
                        {
                            anim.SetTrigger("fireShot");
                            Instantiate(bullet, bulletPoint.transform.position, bulletPoint.transform.rotation);
                        }
                        else
                        {
                            shotWaitCounter=waitBetweenShoot;
                        }
                    
                }
                agent.destination=transform.position;
            }
            else
            {
                shotWaitCounter=waitBetweenShoot;
            }
                }
                anim.SetBool("isMoving", false);
            }
            
        }
      
    }
}
