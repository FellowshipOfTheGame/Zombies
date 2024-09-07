using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponsNS;

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
    private Inventory inventory;
    private WeaponScript weaponScript;
    private WeaponInfoStruct weapon;
    private Dictionary<int, WeaponInfoStruct> inventoryDict;
    private Transform muzzle;
    public Camera mainCamera;
    private readonly Vector3 weaponOffset = new(0.35f, -0.4f, 0.5f);
    private int selectedWeapon = 1;
    public int currentWeapon = 1;
    private const int inventorySize = 3;
    
    
    private void Start() //por algum motivo nao [e so colocar um LoadWeapon() no start entao ta com codigo dobrado
    { 
        timerGO.SetActive(false);
        weaponScript = GetComponent<WeaponScript>();
        inventory = GetComponent<Inventory>();
        inventoryDict = inventory.InventoryDictReference; //referencia o dicionario do Inventario

        Transform weaponTF = transform.GetChild(2);
        weapon = transform.GetChild(2).GetComponent<IWeaponDataProvider>().GetWeaponData();
        muzzle = transform.GetChild(2).GetChild(0);
        
        //atualiza a posicao da arma comparando com o transform da camera
        Vector3 weaponPosition = mainCamera.transform.position +
                                 mainCamera.transform.right * weaponOffset.x +
                                 mainCamera.transform.up * weaponOffset.y +
                                 mainCamera.transform.forward * weaponOffset.z;
        weaponTF.transform.position = weaponPosition;
        weaponTF.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f); 
        //{}{} remendo temporario pros prefabs de teste *Quaternion

        transform.GetChild(2).parent = transform.GetChild(0);
        
        weapon.fireTime = 60f / weapon.fireRate;
        inventoryDict.Add(1, weapon); //salva a arma primaria
        weaponScript.UpdateWeapon(weapon, muzzle);
        
        weaponInfoText.text = weapon.weaponName + " - " + weapon.caliber;
        ammoText.text = weapon.ammo.ToString("D2") + "/"; //interage com o HUD
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        magSizeText.text = weapon.magSize.ToString("D2");
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) LoadWeapon(); //{}{}
        
        if (isSwitching) return;
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventoryDict.Count >= 2) selectedWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventoryDict.Count >= 3) selectedWeapon = 3;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            --selectedWeapon;
            if (selectedWeapon < 1) selectedWeapon = inventoryDict.Count;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ++selectedWeapon;
            if (selectedWeapon > inventoryDict.Count) selectedWeapon = 1;
        }
        if (currentWeapon != selectedWeapon)
        {
            switchingC = StartCoroutine(SwitchWeapon(false));
        }
    }


    private void LoadWeapon()
    {
        Transform newWeapon = transform.GetChild(2); //pega a arma nova
        // var script = child.GetComponent<IWeaponDataProvider>();
        // WeaponInfoStruct data = script.GetWeaponData(); //pega as informacoes da arma
        WeaponInfoStruct newWeaponInfo = newWeapon.GetComponent<IWeaponDataProvider>().GetWeaponData();
        newWeaponInfo.fireTime = 60f / newWeaponInfo.fireRate;

        //atualiza a posicao da arma comparando com o transform da camera
        Vector3 weaponPosition = mainCamera.transform.position +
                                 mainCamera.transform.right * weaponOffset.x +
                                 mainCamera.transform.up * weaponOffset.y +
                                 mainCamera.transform.forward * weaponOffset.z;
        newWeapon.transform.position = weaponPosition;
        newWeapon.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f); 
                                                        //{}{} remendo temporario pros prefabs de teste *Quaternion

        if (inventoryDict.Count < inventorySize) //adiciona mais armas no inventario se tiver poucas
        {
            selectedWeapon = inventoryDict.Count + 1; //indice pra adicionar uma arma nova
            newWeapon.transform.parent = transform.GetChild(0);

            inventoryDict.Add(selectedWeapon, newWeaponInfo);
            switchingC = StartCoroutine(SwitchWeapon(false));
        }
        else if (currentWeapon != 1)
        {
            //FAZER VERIFICACAO SE TA NA ARMA PRIMARIA E NAO DEIXAR COMPRAR {}{}
            Destroy(transform.GetChild(0).GetChild(currentWeapon).gameObject);
            newWeapon.transform.parent = transform.GetChild(0);
            newWeapon.transform.SetSiblingIndex(currentWeapon);

            inventoryDict[currentWeapon] = newWeaponInfo;
            weapon = newWeaponInfo;
            switchingC = StartCoroutine(SwitchWeapon(true));

        }
    }


    IEnumerator SwitchWeapon(bool fullInv)
    {
        isSwitching = true;
        weaponScript.isSwitching = true;
        weaponScript.Stop();

        if (!fullInv) //salva as informacoes da arma, caso tenha inventario cheio nao salva pra deletar o que tinha
        {
            Transform currentWeaponT = transform.GetChild(0).GetChild(currentWeapon);
            currentWeaponT.gameObject.SetActive(false); //esconde a arma atual
            inventoryDict[currentWeapon] = weaponScript.weapon; //salva a arma atual no inventario
        }
        
        float switchTime = weapon.switchTime;
        timerGO.SetActive(true);
        timerSlider.maxValue = weapon.switchTime;
        while (switchTime >= 0)
        {
            timerSlider.value = switchTime;
            switchTime -= Time.deltaTime;
            yield return null;
        }
        timerGO.SetActive(false);

        Transform selectedWeaponT = transform.GetChild(0).GetChild(selectedWeapon); 
        selectedWeaponT.gameObject.SetActive(true); //pega o transform novo liga
        
        weapon = inventoryDict[selectedWeapon]; //carrega os valores da arma no ambiente de trabalho
        currentWeapon = selectedWeapon; //atualiza o indice da arma atual
        
        weaponInfoText.text = weapon.caliber + " - " + weapon.weaponName;
        ammoText.text = weapon.ammo.ToString("D2") + "/";
        totalAmmoText.text = weapon.totalAmmo.ToString("D3");
        magSizeText.text = weapon.magSize.ToString();
        
        muzzle = selectedWeaponT.GetChild(0);
        weaponScript.UpdateWeapon(weapon, muzzle);
        isSwitching = false;
        weaponScript.isSwitching = false;
    }
    
    public void Stop()
    {
        if (isSwitching) StopCoroutine(switchingC);
    }
}
