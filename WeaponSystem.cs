using UnityEngine;

public enum WeaponTypes { MachineGun, GuidedMissile, LazerBullet, TankCanon, Shotgun ,Batarang}
public class WeaponSystem : MonoBehaviour
{
    public WeaponTypes weaponTypes = WeaponTypes.MachineGun;

    [Header("Pickup System")]
    public bool isPickupWeapon = false;
    public float PickupWeaponDisableTime = 15f;
    public GameObject CrospondingGameObject;

    [Header("MachineGun")]
    public Transform[] FirePoints;
    public Transform FireDirection, FireDirection2;
    public bool DoubleFire = false;

    [Header("Guided Missile")]
    public Transform GuidedMissileFirePoint;

    [Header("Canon")]
    public Transform CanonFirePoint;
    public Transform CanonFireDirection;

    [Header("Shotgun")]
    public Transform ShotgunFirePoint;
    public Transform ShotgunFireDirection;

    public GameObject Bullet;
    public GameObject FireEffectParticles;
    public GameObject HitExplosionParticles;
    
    public float FireDelay = 0.25f;
    public float FireForce = 1000f;
    public float AutoDestroyTime = 10f;
    public int Damage = 50;
    public string HitTag;
    public AudioClip ShotSoundClip;
    public AudioSource audioSource;

    [HideInInspector]
    public bool DoFire = false;
    [HideInInspector]
    public bool isWeaponPicked = false;

    private int currentFirePointIndex = 0;
    private float timeTrack = 0;
    private float weaponDisableTime = 0;
    private bool isMissileMode = false;

    public Transform playerTrans;
   public Animator playerAnim;

    [Header("Batarng")]
    public Transform hand;
    public GameObject batarang_prefabs;
    // Start is called before the first frame update
    void Start()
    {
        //enemies = EnemiesSpawner.Instance.AllEnemies;
        
    }

    private void OnEnable()
    {
        if (weaponTypes == WeaponTypes.GuidedMissile)
        {
            isMissileMode = true;
        }
    }

    private void OnDisable()
    {
        isMissileMode = false;
        if (weaponTypes == WeaponTypes.GuidedMissile)
        {
            //for (int a = 0; a < enemies.Length; a++)
            //{
            //    if (enemies[a] != null)
            //        enemies[a].TargetIcon(false);
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPickupWeapon)
        {
            FireHotWeapon();
        }else if (isWeaponPicked)
        {
            FireHotWeapon();
        }
        if (isWeaponPicked) {
            if (weaponDisableTime < Time.time)
            {
                isWeaponPicked = false;
                CrospondingGameObject.SetActive(false);
                GamePlayUIManager.Instance.carRocketButton.SetActive(false);
            }
        }
        if (weaponTypes == WeaponTypes.GuidedMissile)
        {
            UpdateTarget();
        }
    }

    public void EnableWeapon ()
    {
        isWeaponPicked = true;
        weaponDisableTime = PickupWeaponDisableTime + Time.time;
        CrospondingGameObject.SetActive(true);
        GamePlayUIManager.Instance.carRocketButton.SetActive(true);
    }

    void FireHotWeapon ()
    {

        if (Time.time > timeTrack)
        {
            
            switch (weaponTypes)
            {
                case WeaponTypes.MachineGun:
                    if (CustomInput.machineFire)
                        MachineGunFire();
                    break;

                case WeaponTypes.Batarang:
                    
                    if (CustomInput.ThrowBatarang)
                        ThrowBatarang();
                    break;
                case WeaponTypes.LazerBullet:
                    if (CustomInput.laserFire)
                        LaserBullet();
                    break;
                case WeaponTypes.TankCanon:
                    if (CustomInput.canonFire)
                        CanonFire();
                    break;
                case WeaponTypes.GuidedMissile:
                    if (CustomInput.guidedMissile)
                        GuidedMissileFire();
                    break;
                case WeaponTypes.Shotgun:
                    if (CustomInput.Shotgunfire)
                        ShotgunFire();
                    break;
            }
        }
    }

    public void ShotgunFire ()
    {
        GameObject bulletObject = Instantiate(Bullet, 
            ShotgunFirePoint.position, 
            ShotgunFireDirection.rotation);
        GameObject MuzzleFlash = Instantiate(FireEffectParticles, 
            ShotgunFirePoint.position, 
            ShotgunFireDirection.rotation);
        MuzzleFlash.transform.SetParent(ShotgunFirePoint);
        audioSource.PlayOneShot(ShotSoundClip);
        FireBullet(bulletObject);
        timeTrack = Time.time + FireDelay;
    }
    public void ThrowBatarang()
    {
        GameObject Batarang= Instantiate(batarang_prefabs,
            hand.position,
            ShotgunFireDirection.rotation);
        GameObject MuzzleFlash = Instantiate(FireEffectParticles,
            hand.position,
            ShotgunFireDirection.rotation);
        MuzzleFlash.transform.SetParent(ShotgunFirePoint);
        audioSource.PlayOneShot(ShotSoundClip);
        FireBatarang(Batarang);
        //currentFirePointIndex++;
        timeTrack = Time.time + FireDelay;
    }
    private void FireBatarang(GameObject bulletgameobject)
    {
        Bullet bullet = bulletgameobject.GetComponent<Bullet>();
        bullet.HitTag = HitTag;

        if (weaponTypes != WeaponTypes.Shotgun)
        {
           
        }
        Rigidbody rig = bulletgameobject.GetComponent<Rigidbody>();
        rig.AddForce(bulletgameobject.transform.forward *FireForce);

        bullet.ExplosionObject = HitExplosionParticles;
        bullet.DamageTake = Damage;
        Destroy(bullet, AutoDestroyTime);
    }
    public void GuidedMissileFire ()
    {
        GameObject GMissile = Instantiate(Bullet, GuidedMissileFirePoint.position, GuidedMissileFirePoint.rotation);
        Instantiate(FireEffectParticles, GuidedMissileFirePoint.position, GuidedMissileFirePoint.rotation);
        audioSource.PlayOneShot(ShotSoundClip);

        GuidedMissile missle = GMissile.GetComponent<GuidedMissile>();
        missle.Target = MissileTarget;
        missle.ActivateMissile();
        missle.HitTag = HitTag;
        missle.DamageTake = Damage;
        Destroy(missle, AutoDestroyTime);
        timeTrack = Time.time + FireDelay;
    }

    public void CanonFire ()
    {
        GameObject bulletObject = Instantiate(Bullet, CanonFirePoint.position, CanonFireDirection.rotation);
        GameObject MuzzleFlash = Instantiate(FireEffectParticles, CanonFirePoint.position, CanonFireDirection.rotation);
        MuzzleFlash.transform.SetParent(CanonFirePoint);
        audioSource.PlayOneShot(ShotSoundClip);
        FireBullet(bulletObject);
        currentFirePointIndex++;
        timeTrack = Time.time + FireDelay;
    }

    public void LaserBullet ()
    {
        if (currentFirePointIndex == FirePoints.Length)
            currentFirePointIndex = 0;
        GameObject bulletObject = Instantiate(Bullet, FirePoints[currentFirePointIndex].position, FireDirection.rotation);
        GameObject MuzzleFlash = Instantiate(FireEffectParticles, FirePoints[currentFirePointIndex].position, FireDirection.rotation);
        MuzzleFlash.transform.SetParent(FirePoints[currentFirePointIndex]);
        audioSource.PlayOneShot(ShotSoundClip);
        FireBullet(bulletObject);

        currentFirePointIndex++;
        timeTrack = Time.time + FireDelay;
    }
    public void MachineGunFire ()
    {
        GameObject MuzzleFlash;
        if (DoubleFire)
        {
            GameObject[] Bullets = new GameObject[FirePoints.Length];
            for (int a = 0; a < FirePoints.Length; a++)
            {
                if (a == 0)
                {
                    Bullets[a] = Instantiate(Bullet, FirePoints[a].position, FireDirection.rotation);
                    MuzzleFlash = Instantiate(FireEffectParticles, FirePoints[a].position, FireDirection.rotation);
                }
                else
                {
                    Bullets[a] = Instantiate(Bullet, FirePoints[a].position, FireDirection2.rotation);
                    MuzzleFlash = Instantiate(FireEffectParticles, FirePoints[a].position, FireDirection2.rotation);
                }
                audioSource.PlayOneShot(ShotSoundClip);
                MuzzleFlash.transform.SetParent(FirePoints[a]);
                FireBullet(Bullets[a]);
            }
        }
        else
        {
            if (currentFirePointIndex == FirePoints.Length)
                currentFirePointIndex = 0;
            GameObject bulletObject = Instantiate(Bullet, FirePoints[currentFirePointIndex].position, FireDirection.rotation);
            MuzzleFlash = Instantiate(FireEffectParticles, FirePoints[currentFirePointIndex].position, FireDirection.rotation);
            MuzzleFlash.transform.SetParent(FirePoints[currentFirePointIndex]);
            audioSource.PlayOneShot(ShotSoundClip);
            FireBullet(bulletObject);
        }
        

        currentFirePointIndex++;
        timeTrack = Time.time + FireDelay;
    }

    private void FireBullet (GameObject bulletgameobject)
    {
        Bullet bullet = bulletgameobject.GetComponent<Bullet>();
        bullet.HitTag = HitTag;
        
        if (weaponTypes != WeaponTypes.Shotgun)
        {
            Rigidbody rig = bulletgameobject.GetComponent<Rigidbody>();
            rig.AddForce(bulletgameobject.transform.forward * FireForce);
        }
        
        bullet.ExplosionObject = HitExplosionParticles;
        bullet.DamageTake = Damage;
        Destroy(bullet, AutoDestroyTime);
    }

    
    public Transform MissileTarget;
    Enemy[] enemies;
    //public Transform FindClosestEnemy ()
    //{
    //    Transform target = null;

    //    if (enemies.Length > 0)
    //    {
    //        for (int a = 0; a < enemies.Length; a++)
    //        {
    //            if (enemies[a].gameObject.activeInHierarchy)
    //            {
    //                Vector3 forward = playerTrans.transform.TransformDirection(Vector3.forward);
    //                Vector3 toOther = enemies[a].transform.position - transform.position;
    //                if (Vector3.Dot(forward, toOther) > 0.9f)
    //                {
    //                    target = enemies[a].MissileTarget;
    //                    enemies[a].TargetIcon(true);
    //                    return target;
    //                }
    //                else
    //                {
    //                    enemies[a].TargetIcon(false);
    //                }
    //            }
    //        }
            
    //        return target;
    //    }
    //    else
    //    {
    //        return null;
    //    }

    //    //if (enemies.Length > 0)
    //    //{
    //    //    for (int a = 0; a < enemies.Length; a++)
    //    //    {
    //    //        enemies[a].TargetIcon(false);
    //    //        if (enemies[a].inCameraView)
    //    //        {
    //    //            target = enemies[a].MissileTarget;
    //    //            selectedID = a;
    //    //        }
    //    //    }
    //    //    enemies[selectedID].TargetIcon(true);
    //    //    return target;
    //    //}else
    //    //{
    //    //    return null;
    //    //}
    //}

    public void UpdateTarget ()
    {
        //if (MissileTarget == null || !MissileTarget.gameObject.activeInHierarchy)
        //{
            //MissileTarget = FindClosestEnemy();
            //print("Updating Target");
        //}
    }

    public void OnPressFire()
    {
        DoFire = true;
    }

    public void OnReleaseFire()
    {
        DoFire = false;
    }


}