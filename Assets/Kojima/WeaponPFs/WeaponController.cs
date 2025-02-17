using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    [Header("Variables")]
    private bool isReloading;
    private bool isShooting;
    private bool isSwitching;
    private bool partial;
    private const float fireModeSwitchTime = 0.2f;
    
    [Header("References")] 
    [SerializeField] private AudioClip  shootSound;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private WeaponTemplate template;  // add the weapon's template in Unity's spector
    private Transform muzzle;
    private Transform mainCamera;

    [Header("Declarations")]
    public WeaponStruct data;
    public Coroutine timerC;
    private Coroutine reloadingC;
    private Coroutine shootingC;
    private PlayerHUD playerHUD;
    private AudioSource audioSource;
    // usado pra definir qual funcao chamar ao atirar ao inves de usar ifs ou switches
    private delegate IEnumerator FireDelegate();
    private FireDelegate shootFunction; 
    private enum FireMode
    {
        FullAuto,
        Burst,
        SemiAuto
    }
    private FireMode currentFireMode;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        data = template.data;
        data.fireTime = 60f/template.data.fireRate;
        muzzle = transform.GetChild(0);
        
        // currentFireMode = o modo com mais tiros possivel
        currentFireMode = data.isFullAuto ? FireMode.FullAuto : 
            data.burstSize > 1 ? FireMode.Burst : FireMode.SemiAuto;
        SetShootFunction(currentFireMode);
        
        Debug.Log(data.weaponName+" "+currentFireMode);
    }
    

    private void Update()
    {
        if (isReloading) return;

        if (data.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        { partial = false; Reload(); }  //empty reload
        
        if (Input.GetKeyDown(KeyCode.R) && data.ammo < data.magSize)
        { partial = true; Reload(); }  //manual reload
        
        if (Input.GetKeyDown(KeyCode.C) && data.hasFireSelector)
        {
            isSwitching = true;
            if (isShooting) StopCoroutine(shootingC); isShooting = false;
            timerC = StartCoroutine(playerHUD.Timer(fireModeSwitchTime, fireModeSwitchTime,
                CycleFireMode));
        }
        
        //checar isReloading duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isReloading || isShooting || isSwitching) { return; }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        { shootingC = StartCoroutine(shootFunction()); }
        
        // if (Input.GetKeyDown(KeyCode.Mouse1))
        // { mirar }
    }
    
    private void CycleFireMode()
    {
        currentFireMode = currentFireMode switch
        {
            FireMode.FullAuto => data.burstSize > 1 ? FireMode.Burst : FireMode.SemiAuto,
            FireMode.Burst    => FireMode.SemiAuto,
            FireMode.SemiAuto => FireMode.FullAuto,
            _ => currentFireMode
        };

        SetShootFunction(currentFireMode);
    }
    
    private void SetShootFunction(FireMode mode)
    {
        shootFunction = mode switch
        {
            FireMode.FullAuto => ShootFullAuto,
            FireMode.Burst    => ShootBurst,
            FireMode.SemiAuto => ShootSemiAuto,
            _                 => null
        };
        isSwitching = false;
    }
    
    private IEnumerator ShootFullAuto()
    {
        isShooting = true;
        while (data.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) //enquanto tiver municao e continuar atirando
        {
            Shoot();
            yield return new WaitForSeconds(data.fireTime);
        }
        isShooting = false;
    }
    
    private IEnumerator ShootBurst()
    {
        isShooting = true; int i = 0;
        while (data.ammo > 0 && i < data.burstSize)
        {
            Shoot(); ++i;
            yield return new WaitForSeconds(data.fireTime);
        }
        isShooting = false;
    }

    private IEnumerator ShootSemiAuto()
    {
        isShooting = true;
        Shoot();
        yield return new WaitForSeconds(data.fireTime);
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
        if (isShooting) StopCoroutine(shootingC); isShooting = false;
        timerC = StartCoroutine(playerHUD.Timer(partial ? data.reloadTimePartial : data.reloadTime,
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
        isShooting = false;
        isReloading = false;
        isSwitching = false;
    }


    public void Stop()
    {
        if (isShooting)  StopCoroutine(shootingC);  isShooting = false;
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
    }

}
