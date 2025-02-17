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

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private int selectedWeapon = 1;
    [SerializeField] private int currentWeapon = 1;
                     private bool isSwitching;
                     private const int inventorySize = 3;
                     private readonly Vector3 weaponOffset = new(0.35f, -0.4f, 0.5f);  // offset visual pra arma

    [Header("References")] 
    [SerializeField] private Camera mainCamera;
                     private Transform inventory;
    
    [Header("Declarations")]
    private PlayerHUD playerHUD;
    private WeaponController activeWeaponWC;
    
    
    private void Start()
    {
        playerHUD = GetComponent<PlayerHUD>();
        inventory = mainCamera.transform;
        
        SaveWeapon(1);
        SwitchWeapon();
        
        int i = 2;
        while (transform.childCount > 2)
        {
            SaveWeapon(i);
            inventory.GetChild(i).gameObject.SetActive(false);
            ++i;
        }
    }
    
    
    private void Update()
    {
        if (isSwitching) return;
        
        if (Input.GetKeyDown(KeyCode.Equals)) LoadWeapon(); //{}{}
        if (Input.GetKeyDown(KeyCode.T) && inventory.childCount > 2)
        {
            if (currentWeapon == 1)
            {
                selectedWeapon = Random.Range(2, inventory.childCount);
            }
            else
            {
                ThrowWeapon();
            }
        }
        
        //se apertou pra trocar de arma
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.childCount >= 2) selectedWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.childCount >= 3) selectedWeapon = 3;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            --selectedWeapon;
            if (selectedWeapon < 1) { selectedWeapon = inventory.childCount - 1; }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ++selectedWeapon;
            if (selectedWeapon > inventory.childCount- 1) { selectedWeapon = 1; }
        }
        // inventorySize < selectedWeapon < 1
        //se da pra trocar de arma
        if (currentWeapon != selectedWeapon && !isSwitching) { SwitchWeapon(); }
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
            SwitchWeapon();
        }
    }

    private void SaveWeapon(int index)
    {
        Transform newWeapon = transform.GetChild(2); //pega a arma nova
        newWeapon.GetComponent<Rigidbody>().isKinematic = true;

        //atualiza a posicao da arma comparando com o transform da camera
        Vector3 weaponPosition = mainCamera.transform.position + weaponOffset;
        
        // testar isso se nao estiver considerando a rotacao em coordenadas locais da camera
        // Vector3 weaponPosition = mainCamera.transform.position +
                                 // mainCamera.transform.right * weaponOffset.x +
                                 // mainCamera.transform.up * weaponOffset.y +
                                 // mainCamera.transform.forward * weaponOffset.z;
        
        newWeapon.transform.position = weaponPosition;
        // quaternion euler pra arrumar a rotacao do prefab de teste
        newWeapon.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f); 
        
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
        isSwitching = true;
        activeWeaponWC = GetComponentInChildren<WeaponController>();
        inventory.GetChild(currentWeapon).gameObject.SetActive(false);
        currentWeapon = selectedWeapon;
        StartCoroutine(playerHUD.Timer(activeWeaponWC.data.switchTime, 
            activeWeaponWC.data.switchTime, SwitchWeaponFinished));
    }
    
    private void SwitchWeaponFinished()
    {
        inventory.GetChild(selectedWeapon).gameObject.SetActive(true);
        activeWeaponWC = GetComponentInChildren<WeaponController>();
        activeWeaponWC.enabled = true;
        activeWeaponWC.UpdatePlayerHUD();
        isSwitching = false;
    }
    
    private void ThrowWeapon()
    {
        isSwitching = true;
        Transform thrownWeapon = inventory.GetChild(currentWeapon);
        if (activeWeaponWC.timerC !=  null) { StopCoroutine(activeWeaponWC.timerC); }
        thrownWeapon.SetParent(null);
        activeWeaponWC.enabled = false;
        
        Rigidbody thrownWeaponRB = thrownWeapon.GetComponent<Rigidbody>();
        thrownWeaponRB.isKinematic = false;
        thrownWeaponRB.AddForce(mainCamera.transform.forward * 2f);
        
        selectedWeapon = inventory.childCount - 1;
        currentWeapon = selectedWeapon;
        StartCoroutine(playerHUD.Timer(activeWeaponWC.data.switchTime, 
            activeWeaponWC.data.switchTime, SwitchWeaponFinished));
    }
    
    // kinematic = true: voce quem mexe o objeto pelo script ou parents, o sistema de fisica da unity nao interage
    // kinematic = false: a fisica da unity interage com o objeto

    // public void Stop()
    // {
    //     if (isSwitching) StopCoroutine(switchingC);
    // }
}
