using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeapon;
    private int currentWeapon;
    private bool isSwitching;
    private const int inventorySize = 3;
    
    private readonly Vector3 weaponOffset = new(0.35f, -0.4f, 0.5f);  // offset visual pra arma
    private readonly List<GameObject> inventory = new();
    
    private PlayerHUD playerHUD;
    private WeaponController weaponController;
    
    
    private void Start()
    {
        playerHUD = GetComponent<PlayerHUD>();

        while (transform.childCount > 2)  // filhos do player: camera, hud e armas
        {
            AddToInventory(transform.GetChild(2).gameObject);
            inventory[^1].SetActive(false);
        }
        
        SwitchWeapon();  // seleciona a arma primaria
    }
    
    
    private void Update()
    {
        if (isSwitching) return;
        
        if (Input.GetKeyDown(KeyCode.Equals)) AddWeapon(transform.GetChild(2).gameObject);
        if (Input.GetKeyDown(KeyCode.T)) ThrowCurrentWeapon();
        
        //se apertou pra trocar de arma
        if      (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.Count >= 2) selectedWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.Count >= 3) selectedWeapon = 2;
        
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            --selectedWeapon;
            if (selectedWeapon < 0) { selectedWeapon = inventory.Count - 1; }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ++selectedWeapon;
            if (selectedWeapon > inventory.Count- 1) { selectedWeapon = 0; }
        }
        // inventorySize < selectedWeapon < 1
        //se da pra trocar de arma
        if (currentWeapon != selectedWeapon && !isSwitching) { SwitchWeapon(); }
    }

    
    private void SwitchWeapon()
    {
        isSwitching = true;
        inventory[currentWeapon].SetActive(false);
        
        // pega informacoes da arma selecionada
        weaponController = inventory[selectedWeapon].GetComponent<WeaponController>();
        
        // pega informacoes da arma atual, caso nao de pra pegar a info da arma nova
        // weaponController = GetComponentInChildren<WeaponController>();
        
        // usa a funcao de timer do HUD
        StartCoroutine(playerHUD.Timer(weaponController.data.switchTime, 
            weaponController.data.switchTime, EquipWeapon));
    }
    
    
    private void EquipWeapon()
    {
        currentWeapon = selectedWeapon;
        inventory[currentWeapon].SetActive(true);
        weaponController.enabled = true;  // liga o script da arma, ja que script desligado tem que ser manualmente ligado
        weaponController.UpdatePlayerHUD();
        
        isSwitching = false;
    }
    
    
    private void AddWeapon(GameObject weapon)
    {
        if (inventory.Count < inventorySize) { AddToInventory(weapon); }
        else if (currentWeapon == 0) { selectedWeapon = Random.Range(1, inventory.Count); }  // cant throw primary weapon
        else  // but can throw other weapons
        {
            ThrowCurrentWeapon();
            AddToInventory(weapon);
        }
        
        SwitchWeapon();
    }
    
    
    private void AddToInventory(GameObject weapon)
    {
        inventory.Add(weapon);
        ApplyWeaponOffset(weapon);
        selectedWeapon = inventory.Count - 1;
    }
    
    
    private void ApplyWeaponOffset(GameObject weapon)
    {
        // coloca a arma como filho da camera e aplica um offset
        Transform cameraT = transform.GetChild(0);
        weapon.transform.SetParent(cameraT);
        weapon.transform.position = cameraT.position + weaponOffset;
        // quaternion euler pra arrumar a rotacao dos prefabs de teste, tem que mudar depois
        weapon.transform.rotation = cameraT.rotation * Quaternion.Euler(90f, 0f, 0f); 
    }
    
    
    private void ThrowCurrentWeapon()
    {
        if (isSwitching) return;
        
        if (currentWeapon == 0)
        {
            selectedWeapon = Random.Range(1, inventory.Count);
        }  // cant throw primary weapon
        else
        {
            weaponController.enabled = false;
            inventory[currentWeapon].GetComponent<Rigidbody>().isKinematic = false;  // liga a fisica
            inventory[currentWeapon].transform.SetParent(null);
            inventory.RemoveAt(currentWeapon);
            selectedWeapon = inventory.Count - 1;
            currentWeapon = 0;
        }
        
        SwitchWeapon();
    }
    
    
    // kinematic = true: voce quem mexe o objeto pelo script ou parents, o sistema de fisica da unity nao interage
    // kinematic = false: a fisica da unity interage com o objeto
    
}
