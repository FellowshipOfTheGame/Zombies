using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponsNS;
using Random = UnityEngine.Random;


public class WeaponScript : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magSizeText;
    
    [Header("Weapon")]
    public WeaponInfoStruct weapon;
    public AudioClip shootSound;
    public GameObject muzzleFlash;
    private Transform muzzle;

    [Header("Coroutines")] 
    private Coroutine isSwitchingC;
    private Coroutine reloadingC;
    private Coroutine shootingC;
    public bool isSwitching;
    public bool isReloading;
    public bool isShooting;
    
    [Header("Definitions")]
    public AudioSource audioSource;
    public Camera mainCamera;
    
    [Header("Variables")]
    private bool isSpecial = true;
    public string special = "Stack";
    public int percentage = 10;
    
    
    private void Start()
    {
        mainCamera = Camera.main;
        timerGO.SetActive(false);

        ammoText.text = weapon.ammo.ToString("D2") + "/"; //interage com o HUD
        magSizeText.text = weapon.magSize.ToString("D2");
    }

    
    private void Update()
    {
        if (isSwitching || isReloading) return;
        if (weapon.ammo == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R)))
        {
            reloadingC = StartCoroutine(Reload(false)); //empty reload
        }
        if (Input.GetKeyDown(KeyCode.R) && weapon.ammo < weapon.magSize) //manual reload
        {
            reloadingC = StartCoroutine(Reload(weapon.isClosedBolt));
        }
        //checar duas vezes pra nao dar erro de recarregar e atirar ao mesmo tempo
        if (isSwitching || isReloading || isShooting) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
            shootingC = StartCoroutine(weapon.isFullAuto ? ShootingM0() : ShootingM1());
        else if (Input.GetKeyDown(KeyCode.Mouse1)) shootingC = StartCoroutine(ShootingM1());
    }
    
    
    private void Shoot()
    {
        audioSource.PlayOneShot(shootSound);
        GameObject muzzleFlareInstantiate = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
        Destroy(muzzleFlareInstantiate, 0.02f);
        
        for (int i = 0; i < weapon.bulletCount; i++) //atirar varios raycasts se for escopeta
        {
            int damage = weapon.damage;
            float rangeLeft = 3 * weapon.range;
            Vector3 rayOrigin = mainCamera.transform.position;
            Vector3 rayDirection = mainCamera.transform.forward;
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-weapon.spread/2, weapon.spread/2), Random.Range(-weapon.spread/2, weapon.spread/2), 0f);
            rayDirection = spreadRotation * rayDirection;
            
            while (damage > 0) //chain raycasts
            {
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
                {
                    rangeLeft -= hit.distance; //DMG * bullet remaining energy
                    damage -= Mathf.FloorToInt(weapon.damage * hit.distance / (3 * weapon.range));
                    
                    GameObject hitObject = hit.collider.gameObject;
                    if (!hitObject.CompareTag("Player")) break; //se nao acertou um player, para o while
                    
                    if (isSpecial)
                    {
                        hitObject.GetComponent<Interfaces.IDmgSpecial>().
                            TakeDmgSpecial(damage, hit.point, transform, Color.white, special, percentage);
                    }
                    else
                    {
                        hitObject.GetComponent<Interfaces.IDmg>().TakeDmg(damage, hit.point, transform, Color.white);
                    }
                    
                    //prepare to chain raycasts
                    rayOrigin = hit.point + 0.5f*rayDirection; // slight offset to prevent self-collision
                    damage -= weapon.decay;
                }
                else damage = 0;
            }
        }
    }
    
    
    private IEnumerator ShootingM0() //tiro normal / full auto
    {
        isShooting = true;
        while (weapon.ammo > 0 && Input.GetKey(KeyCode.Mouse0)) //se tiver municao e continuar atirando
        {
            Shoot(); --weapon.ammo;
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    private IEnumerator ShootingM1() //tiro alternativo / controlado
    {
        isShooting = true; int i = 0;
        while (weapon.ammo > 0 && i < weapon.burstSize)
        {
            Shoot(); --weapon.ammo; ++i;
            ammoText.text = weapon.ammo.ToString("D2") + "/";
            yield return new WaitForSeconds(weapon.fireTime); //muda o tempo em relacao a fire rate da arma
        }
        isShooting = false;
    }
    
    
    private IEnumerator Reload(bool partial)
    {
        isReloading = true;
        if (isShooting) StopCoroutine(shootingC); isShooting = false;
        
        //show timer slider
        float reloadTime = partial ? weapon.reloadTimePartial : weapon.reloadTime;
        timerGO.SetActive(true);
        timerSlider.maxValue = weapon.reloadTime;
        while (reloadTime >= 0)
        {
            timerSlider.value = reloadTime;
            reloadTime -= Time.deltaTime;
            yield return null;
        }
        timerGO.SetActive(false);
        
        //execute reload logic
        weapon.totalAmmo += weapon.ammo;
        if (weapon.totalAmmo > weapon.magSize) //se tiver bastante municao
        {
            weapon.ammo = partial ? weapon.magSize + 1 : weapon.magSize;
            weapon.totalAmmo -= weapon.ammo;
        }
        else //se tiver pouca municao
        {
            weapon.ammo = weapon.totalAmmo;
            weapon.totalAmmo = 0;
        }
        
        //update HUD
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        isReloading = false;
    }
    
    
    public void UpdateWeapon(WeaponInfoStruct currentWeapon, Transform newMuzzle) //update the selected weapon
    {
        weapon = currentWeapon;
        muzzle = newMuzzle;
    }
    
    
    public void Stop()
    {
        if (isShooting)  StopCoroutine(shootingC);  isShooting = false;
        if (isReloading) StopCoroutine(reloadingC); isReloading = false;
    }
}
