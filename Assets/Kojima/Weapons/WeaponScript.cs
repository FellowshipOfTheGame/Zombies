using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponScript : MonoBehaviour
{
    public WeaponTemplate template;  // add reference in Unity's spector
    public WeaponStruct data;
    private TimerSliderScript timer;
    private AmmoHUDScript ammoHUD;
    
    [Header("Coroutines")]
    private Coroutine switchingC;
    
    [Header("Weapon")]
    public AudioClip  shootSound;
    public GameObject muzzleFlash;
    private Transform muzzle;

    [Header("Coroutines")] 
    private Coroutine reloadingC;
    private Coroutine shootingC;
    public bool isReloading;
    public bool isShooting;
    private bool partial;
    
    [Header("Definitions")]
    public AudioSource audioSource;
    private Transform mainCamera;
    
    
    private void Start()
    {
        mainCamera = transform.parent;
        data = template.data;
        data.fireTime = 60f/template.data.fireRate; 
        //calcula o tempo entre-tiros com base no balas por minuto (fireRate)
        
        timer = GetComponent<TimerSliderScript>();
    }
    

    private void Update()
    {
        if (isReloading) return;
        
        if (data.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        { partial = false; Reload(); }  //empty reload
        
        if (Input.GetKeyDown(KeyCode.R) && data.ammo < data.magSize)
        { partial = true; Reload(); }  //manual reload
        
        //checar isReloading duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isReloading || isShooting) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        { shootingC = StartCoroutine(data.isFullAuto ? ShootAuto() : ShootSingle()); }
        
        // if (Input.GetKeyDown(KeyCode.Mouse1))
        // { mirar }
    }
    
    
    private IEnumerator ShootAuto() //tiro normal / full auto
    {
        isShooting = true;
        while (data.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) //se tiver municao e continuar atirando
        {
            Shoot(); --data.ammo;
            yield return new WaitForSeconds(data.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    private IEnumerator ShootSingle() //tiro alternativo / controlado
    {
        isShooting = true; int i = 0;
        while (data.ammo > 0 && i < data.burstSize)
        {
            Shoot(); --data.ammo; ++i;
            yield return new WaitForSeconds(data.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    private void Shoot()
    {
        audioSource.PlayOneShot(shootSound);
        GameObject muzzleFlareInstantiate = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muzzleFlareInstantiate, 0.02f);
        
        for (int i = 0; i < data.bulletCount; i++) //atirar varios raycasts se for escopeta
        {
            int damage = data.damage;
            float rangeLeft = 3 * data.range;
            Vector3 rayOrigin = mainCamera.transform.position;
            Vector3 rayDirection = mainCamera.transform.forward;
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-data.spread/2, data.spread/2), Random.Range(-data.spread/2, data.spread/2), 0f);
            rayDirection = spreadRotation * rayDirection;
            
            while (damage > 0) //chain raycasts
            {
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
                {
                    rangeLeft -= hit.distance; //DMG * bullet remaining energy
                    damage -= Mathf.FloorToInt(data.damage * hit.distance / (3 * data.range));
                    
                    GameObject hitObject = hit.collider.gameObject;
                    if (!hitObject.CompareTag("Player")) break; //se nao acertou um player, para o while
                    
                    hitObject.GetComponent<Interfaces.IDmg>().TakeDmg(damage, hit.point, transform, Color.white);
                    
                    //prepare to chain raycasts
                    rayOrigin = hit.point + 0.5f*rayDirection; // slight offset to prevent self-collision
                    damage -= data.decay;
                }
                else damage = 0;
            }
        }
        
        ammoHUD.UpdateAmmoText(data.ammo);
    }
    
    
    private void Reload()
    {
        isReloading = true;
        timer.OnTimerComplete += OnTimerFinished;  // evento pro timer usado pro reload
        if (isShooting) StopCoroutine(shootingC); isShooting = false;
        timer.Count(partial ? data.reloadTimePartial : data.reloadTime, data.reloadTime);
    }
    
    private void OnTimerFinished()
    {
        data.totalAmmo += data.ammo;
        if (data.totalAmmo > data.magSize) //se tiver bastante municao
        {
            data.ammo = partial ? data.magSize + 1 : data.magSize;
            data.totalAmmo -= data.ammo;
        }
        else //se tiver pouca municao
        {
            data.ammo = data.totalAmmo;
            data.totalAmmo = 0;
        }
        
        timer.OnTimerComplete -= OnTimerFinished; 
        ammoHUD.UpdateTotalAmmoText(data.totalAmmo);
        ammoHUD.UpdateAmmoText(data.ammo);
        isReloading = false;
        partial = false;
    }


    private void OnDisable()
    {   // se cancelar o reload (como ao trocar de arma), apaga os flags
        partial = false;
        isReloading = false;
        timer.OnTimerComplete -= OnTimerFinished;
    }


    public void Stop()
    {
        if (isShooting)  StopCoroutine(shootingC);  isShooting = false;
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
    }

}
