using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WeaponController : DamageTypes
{
    [Header("Flags")]
    private bool isReloading;
    private bool isShooting;
    private bool isSwitchingModes;
    private bool partialReload;
    
    [Header("FireMode")]
    private string oldFireMode;
    private string newFireMode;
    private string shortFireMode;
    
    [Header("References")]
    [SerializeField] private AudioClip  shootSound;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private WeaponTemplate template;  // add the weapon's template in Unity's spector
    private Transform muzzle;
    public float WeaponSwitchTime => data.weaponSwitchTime; // precisa disso pro WeaponSwitcher pegar a informacao do DamageTypes.data
    
    [Header("Declarations")]
    private Coroutine timerC;
    private Coroutine fireModeC;
    private Coroutine shootingC;
    private PlayerHUD playerHUD;
    
    // [Header("Delegates")]
    private delegate void BulletDelegate();
    private BulletDelegate bulletDelegate;
    private delegate IEnumerator FireDelegate();
    private FireDelegate shootFunction; 
    private enum FireMode
    {
        FullAuto,
        Burst,
        SemiAuto,
        HyperAuto // special hardcoded fire mode for AN94
    }
    private FireMode currentFireMode;
    
    [Header("Pre-caching")]
    private Collider playerCollider;
    private AudioSource audioSource;
    
    
    private void Start()
    {
        data = template.data;
        muzzle = transform.GetChild(0);
        audioSource = GetComponent<AudioSource>();
        bulletDelegate = (data.bulletPrefab == null) ? RaycastBullet : PrefabBullet;
        
        // currentFireMode = o modo com mais tiros possivel
        currentFireMode = data.isFullAuto ? FireMode.FullAuto : 
            data.burstSize > 1 ? FireMode.Burst : FireMode.SemiAuto;
        SetFireMode(currentFireMode);
        
        if (data.ammo == 0) data.ammo = data.magSize;
        if (data.fireTime == 0) data.fireTime = 60f/template.data.fireRate;
        if (data.weaponName == "AN94") currentFireMode = FireMode.HyperAuto;
    }
    
    private new void OnEnable()
    {
        base.OnEnable();
        
        // anything that can change on runtime
        playerHUD = GetComponentInParent<PlayerHUD>();
        playerHUD.UpdateWeaponHUD(data, shortFireMode);
        partialReload = false;
    }
    
    private void Update()
    {
        if (isReloading) return;

        if (data.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        {   // empty reload
            if (timerC != null) { StopCoroutine(timerC); isSwitchingModes = false; }
            partialReload = false; Reload(); 
        }

        if (Input.GetKeyDown(KeyCode.R) && data.ammo < data.magSize)
        {   // manual reload
            if (timerC != null) { StopCoroutine(timerC); isSwitchingModes = false; }
            partialReload = true; Reload(); 
        }
        
        if (isReloading || isSwitchingModes) return;
        if (Input.GetKeyDown(KeyCode.C) && data.hasFireSelector)
        {
            if (isShooting) { StopCoroutine(shootingC); isShooting = false; } 
            
            timerC = StartCoroutine(playerHUD.Timer(data.fireModeSwitchTime, data.fireModeSwitchTime,
                CycleFireMode));
            isSwitchingModes = true;
        }
        
        //checar isReloading duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isReloading || isSwitchingModes || isShooting) return;
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
            FireMode.SemiAuto => data.isFullAuto ? FireMode.FullAuto : FireMode.Burst,
            _                 => currentFireMode
        };
        SetFireMode(currentFireMode);
    }
    
    private void SetFireMode(FireMode mode)
    {
        oldFireMode = newFireMode;
        switch (mode)
        {
            case FireMode.FullAuto:
                shootFunction = ShootFullAuto;
                newFireMode = "Full Auto"; shortFireMode = "FULL";
                break;
            case FireMode.Burst:
                shootFunction = ShootBurst;
                newFireMode = data.burstSize + "-Shot Burst"; shortFireMode = data.burstSize + "-SHOT";
                break;
            case FireMode.HyperAuto: // AN94
                shootFunction = ShootHyper;
                newFireMode = "Hyper Auto"; shortFireMode = "HYPER";
                break;
            default:
            case FireMode.SemiAuto:
                shootFunction = ShootSemiAuto;
                newFireMode = "Semi Auto"; shortFireMode = "SEMI";
                break;
        }
        playerHUD.FireMode(shortFireMode);
        if (fireModeC != null) { StopCoroutine(fireModeC); }
        fireModeC = StartCoroutine(playerHUD.FireModePopUp(oldFireMode, newFireMode)); // pop-up visual
        isSwitchingModes = false;
    }
    
    private IEnumerator ShootFullAuto()
    {
        isShooting = true;
        while (data.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) // enquanto tiver municao e continuar atirando
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
    
    private IEnumerator ShootHyper() // funcao de tiro da AN94
    {
        isShooting = true; 
        Shoot();
        yield return new WaitForSeconds(1f/1300);
        if (data.ammo !=0)
        {
            while (data.ammo > 0 && Input.GetKey(KeyCode.Mouse0))
            {
                Shoot();
                yield return new WaitForSeconds(data.fireTime);
            }
        }
        isShooting = false;
    }
    
    
    private void Shoot()
    {
        audioSource.PlayOneShot(shootSound);
        GameObject muzzleFlareInstantiate = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muzzleFlareInstantiate, 0.02f);
        
        data.ammo--;
        playerHUD.CurrentAmmo(data.ammo);
        for (int i = 0; i < data.bulletCount; i++) bulletDelegate.Invoke();
    }

    private void RaycastBullet()
    {
        int damage = data.damage;
        float rangeLeft = 3 * data.range;
        
        Quaternion spreadRotation = Quaternion.Euler(
            Random.Range(-data.spread/2, data.spread/2), 
            Random.Range(-data.spread/2, data.spread/2), 
            0f);
        
        Vector3 rayOrigin = playerCamera.transform.position;  // tiro sai da camera
        // Vector3 rayOrigin = muzzle.position; // tiro sai da arma
        Vector3 rayDirection = transform.up;  // pros prefabs de teste, essa e a direcao do cano
        rayDirection = spreadRotation * rayDirection;
        
        while (damage >= 0)  // chain raycasts to pierce through enemies
        {
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit target, rangeLeft))
            {
                rangeLeft -= target.distance;
                damage -= Mathf.FloorToInt(data.damage * target.distance/(3 * data.range)); //DMG * bullet remaining energy, arbitrarily 3*range
                
                // if (!target.collider.gameObject.CompareTag("Player") || !target.collider.gameObject.CompareTag("Penetrable")) break;
                if (!target.collider.gameObject.CompareTag("Player")) break; //se nao acertou um player, para o while
                playerHUD.ShowHitmarker();
                
                // pre-caching pq senao repete muita coisa
                DealDamage(target.collider.GetComponent<InterfacesMNG.ICombat>(), target);
                
                //prepare to chain raycasts
                rayOrigin = target.point + 0.5f*rayDirection; // slight offset to prevent self-collision
                damage -= data.decay;
            }
            else break;
        }
        
    }

    private void PrefabBullet()
    {
        GameObject bulletInstance = Instantiate(data.bulletPrefab, muzzle.position, muzzle.rotation);
        bulletInstance.GetComponent<Bullet>().Initialize(20f * transform.up, data.damage, data.sFloat, playerCamera.parent, cExplode);
    }
    
    
    private void Reload()
    {
        isReloading = true;
        if (isShooting) StopCoroutine(shootingC); isShooting = false;
        timerC = StartCoroutine(playerHUD.Timer(partialReload ? data.reloadTimePartial : data.reloadTime,
            data.reloadTime, ReloadFinished));
    }
    
    private void ReloadFinished()
    {
        data.totalAmmo += data.ammo;
        if (data.totalAmmo > data.magSize) //se tiver municao suficiente pra um pente
        {
            data.ammo = partialReload ? data.magSize + 1 : data.magSize;
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
        partialReload = false;
    }

    public void OnEquip()
    {
        playerCamera = transform.parent;
        playerCollider = playerCamera.parent.gameObject.GetComponent<Collider>();
        playerICombat = transform.parent.parent.GetComponent<InterfacesMNG.ICombat>();
    }

    protected override void ExplosiveDamage(InterfacesMNG.ICombat cachedICombat, RaycastHit target)
    {
        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(target.point, data.sFloat));
        
        if (colliders.Contains(playerCollider))
        {
            colliders.Remove(playerCollider);
            DamageCollider(playerCollider, target.point, data.sInt/2); // halve self-damage
        }
        
        foreach (Collider col in colliders) DamageCollider(col, target.point, data.sInt);
    }
    
    private void OnDisable()
    {   // se cancelar o reload (como ao trocar de arma), apaga os flags
        partialReload = false;
        isShooting = false;
        isReloading = false;
    }
    
}
