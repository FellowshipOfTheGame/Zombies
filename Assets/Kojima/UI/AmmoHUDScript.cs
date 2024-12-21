using UnityEngine;
using TMPro;


public class AmmoHUDScript : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    // public TextMeshProUGUI magSizeText;
    // public TextMeshProUGUI weaponInfoText;

    
    public void UpdateAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString("D2") + "/";
    }

    public void UpdateTotalAmmoText(int totalAmmo)
    {
        totalAmmoText.text = totalAmmo.ToString("D3");
    }

    // public void UpdateMagSizeText(int magSize)
    // {
    //     magSizeText.text = magSize.ToString();
    // }
    //
    // public void UpdateWeaponInfoText(string caliber, string weaponName)
    // {
    //     weaponInfoText.text = caliber + " - " + weaponName;
    // }

}
