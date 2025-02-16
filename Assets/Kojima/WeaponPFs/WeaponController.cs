using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    public WeaponTemplate template;  // add the weapon's template in Unity's spector
    public WeaponStruct data;
    private PlayerHUD playerHUD;
    
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
    private AudioSource audioSource;
    private Transform mainCamera;
    
    
    private void Start()
    {
        data = template.data;
        data.fireTime = 60f/template.data.fireRate;
        muzzle = transform.GetChild(0);
        
        audioSource = GetComponent<AudioSource>();
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
        
        if (Input.GetKey(KeyCode.Mouse0))
        { shootingC = StartCoroutine(data.isFullAuto ? ShootAuto() : ShootSingle()); }
        
        // if (Input.GetKeyDown(KeyCode.Mouse1))
        // { mirar }
    }
    
    
    private IEnumerator ShootAuto() //tiro normal / full auto
    {
        isShooting = true;
        while (data.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) //enquanto tiver municao e continuar atirando
        {
            Shoot();
            yield return new WaitForSeconds(data.fireTime);
        }
        isShooting = false;
    }
    
    
    private IEnumerator ShootSingle() //tiro alternativo / controlado
    {
        isShooting = true; int i = 0;
        while (data.ammo > 0 && i < data.burstSize)
        {
            Shoot(); ++i;
            yield return new WaitForSeconds(data.fireTime);
        }
        isShooting = false;
    }
    
    
    private void Shoot()
    {
        audioSource.PlayOneShot(shootSound);
        GameObject muzzleFlareInstantiate = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muzzleFlareInstantiate, 0.02f);
        --data.ammo; playerHUD.CurrentAmmo(data.ammo);
        
        for (int i = 0; i < data.bulletCount; i++) //atirar varios raycasts se for escopeta
        {
            int damage = data.damage;
            float rangeLeft = 3 * data.range;
            
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-data.spread/2, data.spread/2), 
                Random.Range(-data.spread/2, data.spread/2), 
                0f);
            
            Vector3 rayOrigin = mainCamera.transform.position;  // tiro sai da camera
            // Vector3 rayOrigin = muzzle.position;  // tiro sai da arma
            Vector3 rayDirection = transform.up;  // pros prefabs de teste, essa e a direcao do cano
            rayDirection = spreadRotation * rayDirection;
            
            while (damage > 0)  // chain raycasts to pierce through enemies
            {
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
                {
                    rangeLeft -= hit.distance; //DMG * bullet remaining energy
                    damage -= Mathf.FloorToInt(data.damage * hit.distance / (3 * data.range));
                    
                    GameObject hitObject = hit.collider.gameObject;
                    if (!hitObject.CompareTag("Player")) break; //se nao acertou um player, para o while
                    
                    hitObject.GetComponent<InterfacesMNG.IDmg>().TakeDmg(damage, hit.point, mainCamera, Color.white);
                    
                    //prepare to chain raycasts
                    rayOrigin = hit.point + 0.5f*rayDirection; // slight offset to prevent self-collision
                    damage -= data.decay;
                }
                else damage = 0;
            }
        }
    }

    private void Reload()
    {
        isReloading = true;
        if (isShooting) StopCoroutine(shootingC);
        isShooting = false;
        StartCoroutine(playerHUD.Timer(partial ? data.reloadTimePartial : data.reloadTime,
            data.reloadTime, ReloadFinished));
    }
    
    private void ReloadFinished()
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
        
        playerHUD.CurrentAmmo(data.ammo);
        playerHUD.TotalAmmo(data.totalAmmo);
        isReloading = false;
        partial = false;
    }

    public void UpdatePlayerHUD()
    {
        playerHUD = GetComponentInParent<PlayerHUD>();
        playerHUD.CurrentAmmo(data.ammo);
        playerHUD.TotalAmmo(data.totalAmmo);
        playerHUD.MagSize(data.magSize);
        playerHUD.WeaponInfo(data.weaponName, data.caliber);
        mainCamera = transform.parent;
    }
    
    private void OnDisable()
    {   // se cancelar o reload (como ao trocar de arma), apaga os flags
        partial = false;
        isReloading = false;
    }


    public void Stop()
    {
        if (isShooting)  StopCoroutine(shootingC);  isShooting = false;
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
    }

}
