using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponsNS;
using Random = UnityEngine.Random;


public class WeaponScript : MonoBehaviour
{
    // TO-DO
    //  > implementar spread
    //
    //  > colocar barrinha de reload embaixo da crosshair, branco com fundo preto e chanfro vermelho do parcial
    //  
    //  investigar se tem que puxar o bolt pra tras quando acaba a bala de uma closed bolt
    //
    //  fazer verificacao se tem municao o suficiente pra fazer o reload (totalAmmo>magSize)
    //
    //  ta fazendo reload parcial mesmo com a mag vazia
    //
    //  procurar saber como que faz pra ver o tempo que cada script demora pra executar
    //      se esse script for muito pesado, testar otimizar trocando variaveis da página weapon
    //      pra variaveis locais
    //
    //  na hora de separar os scripts, fazer que os scripts das armas estejam na mesma pasta que o ShootingScript
    //      pra que usem o mesmo namespace
    //      ou criar um script so pra definir o namespace e importar nos outros scripts que o usarem
    //
    //  fazer compras de arma terem um cooldown pra nao travar o script de trocar de arma
    //
    // fazer floating number damage indicators !!
    //
    // fazer formula pra aumentar o spread da arma com fogo continuo - log
    //
    // fazer slow ao tomar dano, num ienummerator com while pra aumentar a speed por tempo, assim como o slider aqui
    //
    //  verificar comentarios com {}{}
    //
    // otimizacao do salmaze com varios computadores
    // testar ate quando vale comprimir uma mensagem → testar no load o ping e velocidade de processamento
    // testar de novo a cada [medida de tempo] se tem que atualizar essa medida de processamento/compressao
    //
    // fazer a granada/lanca granada analisar o movimento com um raycast, se bater em alguma coisa,
    //      transform=hit.position, se der pra mover, continua normalmente
    //
    // nao ta pegando as informacoes da arma nova corretamente depois que ta com o inventario cheio

    // a trabalho
    //  nao era pra dar pra recarregar quando totalAmmo==0
    
    
    [Header("UI Elements")]
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magSizeText;
    public WeaponInfoStruct weapon;
    
    [Header("Coroutines")] 
    private Coroutine isSwitchingC;
    private Coroutine reloadingC;
    private Coroutine shootingC;
    public bool isSwitching;
    public bool isReloading;
    public bool isShooting;
    
    [Header("Definitions")]
    public Camera mainCamera;
    private Transform muzzle;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public GameObject muzzleFlash;
    private bool isSpecial = true;
    public string special = "MissingHealth";
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
            
            while (damage > 0) //chain raycasts to 
            {
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rangeLeft))
                {
                    rangeLeft -= hit.distance; //DMG * bullet remaining energy
                    damage -= Mathf.FloorToInt(weapon.damage * hit.distance / (3 * weapon.range));
                    
                    GameObject hitObject = hit.collider.gameObject;
                    if (!hitObject.CompareTag("Player")) break; //se nao acertou um player, para o while
                    
                    if (isSpecial)
                    {
                        hitObject.GetComponent<Interfaces.IDamageSpecial>().TakeDamageSpecial(damage, hit.point, transform, Color.white, special, percentage);
                    }
                    else
                    {
                        hitObject.GetComponent<Interfaces.IDamage>().TakeDamage(damage, hit.point, transform, Color.white);
                    }
                    
                    //prepare to chain raycasts
                    rayOrigin = hit.point - hit.normal; // slightly offset to prevent self-collision
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
