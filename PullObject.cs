using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FightType { PullObject, Batarang}
public class PullObject : MonoBehaviour
{
    [Tooltip("The Point From Where Ray cast to be hit")]
    public Transform rayCastingpoint;
    [Tooltip("Hand that is targt destination of the pull")]
    public Transform hand;
    [Tooltip("tag that is used to determine if an object is pullable or not")]
    public string pullableTag;
    [Tooltip("Force modifie ,tweek it to suity to your needs")]
    public float modifies = 1.0f;
    [Tooltip("the direction of the force that is puling the object ")]
    Vector3 pullForce;
    [Tooltip("Once an object is in the hand ,save it to this variable")]
    public Transform heldObject;
    [Tooltip("The Distance threshold in which the object is considered pulled to the hand")]
    public float positionDistanceThreshold;
    [Tooltip("the Distance threshold is which the object's velocity is set to maximam")]
    public float velocityDistanceTreshold;
    [Tooltip("The maximm velocity of the object being pulled")]
    public float maxVelocity;
    [Tooltip("the velocity at which the object is thrown")]
    public float throwVelocity;
    public float range=50;
    public Animator anim;
    public Button gunPlayBtn,pullObjectBtn;
    public GameObject gun;

    [Header("Line Renderer")]
    public Transform target;
    public int resolution, waveCount, wobbleCount;
    public float waveSize, animSpeed;
    public LineRenderer lineRenderer;
    public Transform gunaim;
    //public int lengthOfLineRenderer = 20;

        [Header("Batarang Work")]
    public Transform batarang_hand;
    public GameObject batarang_prefabs;
    private float timeTrack = 0;
    public float FireDelay = 0.25f;
    public float FireForce = 10000f;
    public float AutoDestroyTime = 10f;
    public int Damage = 50;
    public string HitTag;
    public AudioClip ShotSoundClip,gunSound,powerjump,tranformationsound,grapsound,reloadSound;
    public AudioSource audioSource;
    public GameObject FireEffectParticles;
    public GameObject HitExplosionParticles;
    [Header("Carry")]
    public Transform carry_hand;
    public Button[] ability_Buttons;


    public FightType fightType;
    public static PullObject instance;
  


    private void Awake()
    {
       
        instance = this;
    }
    void Start()
    {
        anim = (Animator)GetComponent("Animator");
        //target = GameObject.FindGameObjectWithTag("Enemy").transform;
        fightType = FightType.Batarang;
        //lineRenderer.widthMultiplier = 0.2f;
        //lineRenderer.positionCount = lengthOfLineRenderer;
    }
    public void Transfer()
    {
        //transform.rotation = PlayerCam.transform.rotation;
    }
   public void GunPlay(bool gunplay)
    {
       
        if (!GetComponent<FlyBehaviour>().fly)
        {

       
            if (gunplay)
            {

                fightType = FightType.PullObject;
                if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Gun"))
                {
                    anim.SetBool("Gunplay", true);
                    ReloadSound();
                    gun.SetActive(true);
                }
                pullObjectBtn.gameObject.SetActive(true);
                pullObjectBtn.GetComponent<TweenScale>().enabled = true;
                gunPlayBtn.onClick.RemoveAllListeners();
                //gunPlayBtn.onClick.AddListener(GunPlay => { false });
                gunPlayBtn.onClick.AddListener(() => { GunPlay(false); });


            }
            else
            {

                pullObjectBtn.gameObject.SetActive(false);
                fightType = FightType.Batarang;
                anim.SetBool("Gunplay", false);
                gun.SetActive(false);
                gunPlayBtn.onClick.AddListener(() => { GunPlay(true); });
            }

        }


    }
    public void ReloadSound()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
    public void BatarangShootSound()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(ShotSoundClip);
        }
    }
    public void GunShootSounds()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(ShotSoundClip);
        }
    }
    public void JumpSound()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(powerjump);
        }
    }
    public void TransformationSound()
    {
        if (audioSource)
        {

            audioSource.PlayOneShot(tranformationsound);
        }
    }
    public void GrapeSound()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(grapsound);
        }
    }
    void Update()
    {

        //var t = Time.time;
        //for (int i = 0; i < lengthOfLineRenderer; i++)
        //{
        //    lineRenderer.SetPosition(i, new Vector3(i * 0.5f, Mathf.Sin(i + t), 0.0f));
        //}
       
        
     
        //Debug.DrawRay(rayCastingpoint.position, transform.TransformDirection(Vector3.forward)*range, Color.yellow);
        if (fightType==FightType.PullObject && !GetComponent<FlyBehaviour>().fly)
        {
            
            RaycastHit hit;
            //Debug.DrawRay(rayCastingpoint.position, rayCastingpoint.transform.TransformDirection(Vector3.forward) * range, Color.yellow);
            if (Physics.Raycast(rayCastingpoint.position, rayCastingpoint.transform.forward, out hit, range))
            {
              

                if (CustomInput.pullObject)
        {


                    pullObjectBtn.GetComponent<TweenScale>().enabled = false;

                    //Debug.DrawRay(rayCastingpoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                    if (hit.transform.tag.Equals(pullableTag))
                {
                     
                    //lineRenderer.enabled = true;
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Shoot"))
                    {
                        anim.CrossFade("Shoot", 0.5f);
                        
                    }
                    StartCoroutine(ObjectToPulled(hit.transform));
                        //StartCoroutine(AnimateRope(hit.transform.position));
                        

                    }
                }
        }
        }
        else if (fightType == FightType.Batarang && !GetComponent<FlyBehaviour>().fly)
        {
            if (CustomInput.ThrowBatarang)
            {
                //anim.applyRootMotion = false;
                if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Batarang"))
                {
                    anim.CrossFade("Batarang", 0.1f);
                    
                    //anim.CrossFade()

                }
            }
        }

        if (CustomInput.pullObject == false)
        {

            if (heldObject != null)
            {
                print("not null");
                heldObject.transform.parent = null;
                heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                heldObject.GetComponent<Rigidbody>().velocity = transform.forward * throwVelocity;
                heldObject = null;
              
            }
        }

    }
    IEnumerator ObjectToPulled(Transform t)
    {
        Rigidbody r = t.GetComponent<Rigidbody>();
       

        while (true)
        {

            if (CustomInput.pullObject==false)
            {
               
                break;
            }
          
            float distanceToHand = Vector3.Distance(t.position, hand.position);
         
            if (distanceToHand < positionDistanceThreshold)
            {
                print("distanceToHand < positionDistanceThreshold");
                t.position = hand.position;
                t.parent = hand;
                r.constraints = RigidbodyConstraints.FreezePosition;
                heldObject = t;
                break;
            }
            Vector3 pullDirection = hand.position - t.position;
            if (distanceToHand > 5)
            {
                
                
                if (r.mass > 20)
                {
                    pullForce = pullDirection.normalized * modifies * 100;
                    //print(pullForce);
                }
                else
                {
                    pullForce = pullDirection.normalized * modifies;
                }
            }
           
                if (r.velocity.magnitude < maxVelocity && distanceToHand > velocityDistanceTreshold)
                {
                //print("r.velocity.magnitude===" + r.velocity.magnitude);
                //print("maxVelocity===" + maxVelocity);
                //print("distanceToHand===" + distanceToHand);
                //print("velocityDistanceTreshold===" + velocityDistanceTreshold);

                r.AddForce(pullForce, ForceMode.Force);
              
                //////t.position = hand.position;
                //pullForce = pullForce*0;

            }
                else
                {
                    print("r vlcy mag grter the max vlchty");
                    r.velocity = pullDirection.normalized * maxVelocity;
                }

           
            yield return null;
        }
    }

    IEnumerator AnimateRope(Vector3 targetPos)
    {
        lineRenderer.positionCount = resolution;
        float angle = LookatAngle(targetPos - gunaim.position);
        float percent = 0;
        while (percent <= 1f)
        {
            percent += Time.deltaTime * animSpeed;
            SetPoints(targetPos, percent, angle);
            yield return null;
        }

        SetPoints(targetPos, 1, angle);
    }

    private void SetPoints(Vector3 targetPos, float percent, float angle)
    {
        Vector3 ropeEnd = Vector3.Lerp(gunaim.position, targetPos, percent);
        float length = Vector3.Distance(gunaim.position, ropeEnd);
        for (int i = 0; i < resolution; i++)
        {
            float xPos = (float)i / resolution * length;
            float reversePercentn = (i - percent);
            float amplitude = Mathf.Sin(reversePercentn * wobbleCount * Mathf.PI) * ((1f - (float)i / resolution) * waveSize);
            float zPos = Mathf.Sin((float)waveCount * i / resolution * 2 * Mathf.PI * reversePercentn) * amplitude;
            Vector3 pos = RotatePoint(new Vector3(xPos + gunaim.position.x, zPos + gunaim.transform.position.z), gunaim.position, angle);
            lineRenderer.SetPosition(i, targetPos);
        }
    }

    private Vector3 RotatePoint(Vector3 point, Vector3 pivot, float angle)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(0, 0, angle) * dir;
        point = dir + pivot;
        return point;
    }

    private float LookatAngle(Vector3 target)
    {
        return Mathf.Atan2(target.z, target.x) * Mathf.Rad2Deg;
    }

    #region Batarang
    public void ThrowBatarang()
    {
        GameObject Batarang = Instantiate(batarang_prefabs,
           batarang_hand.position,
            this.transform.rotation);
        GameObject MuzzleFlash = Instantiate(FireEffectParticles,
            hand.position,
            this.transform.rotation);
        MuzzleFlash.transform.SetParent(this.transform);
        //audioSource.PlayOneShot(ShotSoundClip);
        FireBatarang(Batarang);
        //currentFirePointIndex++;
        timeTrack = Time.time + FireDelay;
        anim.applyRootMotion = true;
    }
    private void FireBatarang(GameObject bulletgameobject)
    {
        Bullet bullet = bulletgameobject.GetComponent<Bullet>();
        bullet.HitTag = HitTag;

        
        Rigidbody rig = bulletgameobject.GetComponent<Rigidbody>();
        rig.AddForce(gameObject.transform.forward * FireForce);

        bullet.ExplosionObject = HitExplosionParticles;
        bullet.DamageTake = Damage;
        Destroy(bullet, AutoDestroyTime);
    }
    #endregion

    #region Carry 

    public void Carry(GameObject obj)
    {
        //fightType = FightType.Carry;
        foreach (var item in ability_Buttons)
        {
            item.GetComponent<Image>().raycastTarget= false;
            item.interactable= false;
        }
        anim.SetBool("Carry", true);
        obj.transform.parent = carry_hand.transform;
        obj.transform.position = carry_hand.transform.position;
        obj.transform.rotation = carry_hand.transform.rotation;
        //obj.transform.localScale = carry_hand.transform.localScale;

    }
    #endregion

    #region Particls
  


    #endregion
}
