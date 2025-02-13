using UnityEngine;

/// <summary>
///     Start()
///         desliga o elemento de hud do timer
///         carrega as armas do player
///         atualiza as informacoes no HUD
/// 
///     Update()
///         verifica inputs do usuario
///
///     LoadWeapon()
///         realiza a logica pra pegar uma arma nova
///
///     SaveWeapon()
///         salva a arma nova no inventario e aplica o offset na tela
///
///     SwitchWeapon()
///         troca qual arma esta ativa na tela
///
///     ThrowWeapon()
///         descarta a arma atual, caso o jogador aperte T
///         ou pegue uma arma nova com inventario cheio
/// 
/// </summary>

public class SwitchScript : MonoBehaviour
{
    [Header("Coroutines")]
    private Coroutine switchingC;
    private bool isSwitching;
    
    [Header("Definitions")]
    public Camera mainCamera;
    private Transform inventory;
    
    [Header("Variables")]
    private readonly Vector3 weaponOffset = new(0.35f, -0.4f, 0.5f);  // offset de teste pra arma na tela
    public int selectedWeapon = 1;
    public int currentWeapon = 1;
    private const int inventorySize = 3;
    // private float switchTime;
    
    
    private void Start()
    {
        inventory = mainCamera.transform;
        SaveWeapon(1);
        SwitchWeapon();
    }
    
    
    private void Update()
    {
        if (isSwitching) return;
        
        if (Input.GetKeyDown(KeyCode.Equals)) LoadWeapon(); //{}{}
        if (Input.GetKeyDown(KeyCode.T) && currentWeapon != 1)
        {
            ThrowWeapon();
            --selectedWeapon; --currentWeapon;
            SwitchWeapon();
        }
        
        //se apertou pra trocar de arma
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.childCount >= 2) selectedWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.childCount >= 3) selectedWeapon = 3;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            --selectedWeapon;
            Debug.Log("--");
            if (selectedWeapon < 1) selectedWeapon = inventory.childCount - 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ++selectedWeapon;
            Debug.Log("++");
            if (selectedWeapon > inventory.childCount- 1) selectedWeapon = 1;
        }
        // inventorySize < selectedWeapon < 1
        //se da pra trocar de arma
        if (currentWeapon != selectedWeapon)
        {
            SwitchWeapon();
        }
    }

    
    private void LoadWeapon()
    {
        if (inventory.childCount <= inventorySize)  // inventario com espaco
        {
            SaveWeapon(inventory.childCount);
            selectedWeapon = currentWeapon + 1;
        }
        else if (currentWeapon == 1)
        {
            currentWeapon = 2;
        }
        else
        {
            ThrowWeapon();
            SaveWeapon(currentWeapon);
        }
        SwitchWeapon();
    }

    private void SaveWeapon(int index)
    {
        Transform newWeapon = transform.GetChild(2); //pega a arma nova

        //atualiza a posicao da arma comparando com o transform da camera
        Vector3 weaponPosition = mainCamera.transform.position +
                                 mainCamera.transform.right * weaponOffset.x +
                                 mainCamera.transform.up * weaponOffset.y +
                                 mainCamera.transform.forward * weaponOffset.z;
        
        newWeapon.transform.position = weaponPosition;
        newWeapon.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f); 
        //{}{} posicao temporaria pros prefabs de teste * Quaternion
        
        newWeapon.SetParent(inventory);
        newWeapon.SetSiblingIndex(index);
    }

    // private void PickWeapon()
    // {
    //     //remove rb
    //     LoadWeapon();
    // }

    private void SwitchWeapon()
    {
        inventory.GetChild(currentWeapon).gameObject.SetActive(false);
        //timer
        inventory.GetChild(selectedWeapon).gameObject.SetActive(true);

        currentWeapon = selectedWeapon;
    }

    private void ThrowWeapon()
    {
        Transform thrownWeapon = inventory.GetChild(currentWeapon);
        thrownWeapon.SetParent(null);
        // Rigidbody thrownWeaponRB = thrownWeapon.GetComponent<Rigidbody>();  // se ja tiver o RB na arma
        // thrownWeaponRB.useGravity = true;
        thrownWeapon.gameObject.AddComponent<Rigidbody>();
        Rigidbody thrownWeaponRB = thrownWeapon.GetComponent<Rigidbody>();
        thrownWeaponRB.AddForce(mainCamera.transform.forward * 2f);
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
    
    // public void Stop()
    // {
    //     if (isSwitching) StopCoroutine(switchingC);
    // }
}
