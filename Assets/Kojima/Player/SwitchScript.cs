using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///
///     Dependencias:
///     esse script depende dos outros scripts de arma: Inventory e Weapon
///
///     Funcao do script:
///     Esse script se encarrega de adicionar novas armas ao Inventory e trocar
/// qual arma esta ativa, atualizando as respectivas informacoes no HUD.
///     Ao adicionar uma arma nova, ela [e automaticamente equipada e caso o
/// Inventory esteja cheio, implementado como 3 armas, a arma que nao seja
/// a primaria sera trocada.
/// 
/// </summary>

public class SwitchScript : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magSizeText;
    public TextMeshProUGUI weaponInfoText;

    [Header("Coroutines")]
    private Coroutine switchingC;
    private bool isSwitching;
    
    [Header("Definitions")]
    public Camera mainCamera;
    private Transform inventory;
    private Transform muzzle;
    private WeaponScript weaponScript;
    
    [Header("Variables")]
    private readonly Vector3 weaponOffset = new(0.35f, -0.4f, 0.5f);  // offset de teste pra arma na tela
    private int selectedWeapon = 1;
    public int currentWeapon = 1;
    private const int inventorySize = 3;
    
    //to-do
    
    //trocar arma
    // desativar arma atual e ativar a nova arma
    // verificar quantas armas tem pelo inventory.childcount
    
    //adicionar armas
    // pegar arma nova e colocar como filho da camera
    
    //descartar armas
    // se tiver com o inventario cheio e nao for a arma 1, ao pegar uma arma nova, tem que descartar a antiga
    
    
    private void Start() //por algum motivo nao [e so colocar um LoadWeapon() no start entao ta com codigo dobrado
    { 
        inventory = mainCamera.transform;
        timerGO.SetActive(false);
        
        //load weapon
        //pega a arma como filha do player e mexe pra filha da camera

        //update UI
        //carrega as informacoes da arma na tela



    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) LoadWeapon(); //{}{}
        
        if (isSwitching) return;
        
        //se apertou pra trocar de arma
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.childCount >= 2) selectedWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.childCount >= 3) selectedWeapon = 3;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            --selectedWeapon;
            if (selectedWeapon < 1) selectedWeapon = inventory.childCount;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ++selectedWeapon;
            if (selectedWeapon > inventory.childCount) selectedWeapon = 1;
        }
        
        //se da pra trocar de arma
        if (currentWeapon != selectedWeapon)
        {
            // switchingC = StartCoroutine(SwitchWeapon());
        }
    }

    
    private void LoadWeapon()
    {
        Transform newWeapon = transform.GetChild(2); //pega a arma nova

        //atualiza a posicao da arma comparando com o transform da camera
        Vector3 weaponPosition = mainCamera.transform.position +
                                 mainCamera.transform.right * weaponOffset.x +
                                 mainCamera.transform.up * weaponOffset.y +
                                 mainCamera.transform.forward * weaponOffset.z;
        newWeapon.transform.position = weaponPosition;
        newWeapon.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f); 
                                                        //{}{} remendo temporario pros prefabs de teste *Quaternion

    }


    // IEnumerator SwitchWeapon()
    // {
    //     isSwitching = true;
    //     weaponScript.Stop();
    //     
    //     if (!fullInv) //salva as informacoes da arma, caso tenha inventario cheio nao salva pra deletar o que tinha
    //     {
    //         Transform currentWeaponT = transform.GetChild(0).GetChild(currentWeapon);
    //         currentWeaponT.gameObject.SetActive(false); //esconde a arma atual
    //         inventoryDict[currentWeapon] = weaponScript.weapon; //salva a arma atual no inventario
    //     }
    //     
    //     float switchTime = data.switchTime;
    //     timerGO.SetActive(true);
    //     timerSlider.maxValue = data.switchTime;
    //     while (switchTime >= 0)
    //     {
    //         timerSlider.value = switchTime;
    //         switchTime -= Time.deltaTime;
    //         yield return null;
    //     }
    //     timerGO.SetActive(false);
    //
    //     Transform selectedWeaponT = transform.GetChild(0).GetChild(selectedWeapon); 
    //     selectedWeaponT.gameObject.SetActive(true); //pega o transform novo liga
    //     
    //     currentWeapon = selectedWeapon; //atualiza o indice da arma atual
    //     
    //     weaponInfoText.text = weapon.caliber + " - " + weapon.weaponName;
    //     ammoText.text = weapon.ammo.ToString("D2") + "/";
    //     totalAmmoText.text = weapon.totalAmmo.ToString("D3");
    //     magSizeText.text = weapon.magSize.ToString();
    //     
    //     muzzle = selectedWeaponT.GetChild(0);
    //     shootScript.UpdateWeapon(weapon, muzzle);
    //     isSwitching = false;
    //     shootScript.isSwitching = false;
    // }
    
    public void Stop()
    {
        if (isSwitching) StopCoroutine(switchingC);
    }
}
