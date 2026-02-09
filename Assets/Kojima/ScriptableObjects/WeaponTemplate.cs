using UnityEngine;

[System.Serializable]
public struct WeaponStruct
{
    [Header("Information")]
    public string weaponName;
    public string caliber;
    
    [Header("Fire Mode")]
    [Tooltip("1 for no burst / 2+ for burst")]
    public int  burstSize; // se for 1, desliga a opcao de burst
    public bool isFullAuto;
    public bool hasFireSelector;
    public float fireModeSwitchTime;
    
    [Header("Stats")]
    public int   damage;
    [Tooltip("How many bullets per shot")]
    public int   bulletCount;
    [Tooltip("Bullets per minute")]
    public float fireRate;
    public int   magSize;
    public int   totalAmmo;
    public float reloadTime;
    [Tooltip("Reload time on a partial reload")]
    public float reloadTimePartial;
    public float weaponSwitchTime;
    [Tooltip("Solid angle (3D cone) of spread")]
    public float spread;
    public float range;
    [Tooltip("Damage reduction when going through things")]
    public int   decay;
    public float recoil;
    
    [Header("Can be empty")]
    [Tooltip("Current ammo in the magazine")]
    public int   ammo;
    public float fireTime;
    [Tooltip("Will use raycast if the bullet prefab is left empty")]
    public GameObject bulletPrefab;
    
    public enum Special
    {
        None,
        Explosive,  // damage= sInt over a sFloat area
        LowHealth,  // damage= up to dmg*sFloat bonus
        HighHealth, // damage= up to dmg*sFloat bonus
        Echo,       // damage= sInt after a sFloat delay
        True,       // damage= 
        Poison,     // damage= sInt stacks with sFloat tickTime
        Bleed,      // damage= sInt damage with sFloat tickTime
        Hemorrhage, // damage= sInt damage with sFloat execute, ticks every .5s
        DoT,        // damage= sInt dps, sFloat time duration, ticks every .2s (sInt/5 /tick)
        Healing,    // damage, and sInt healing
        HoT,        // damage, and sInt healing with sFloat tickTime
    }
    [Header("Special damage")]
    public Special special;
    [Tooltip("damage, stacks")] public int sInt;
    [Tooltip("percentage, range, tick time")] public float sFloat;
}


[CreateAssetMenu(fileName = "Weapon_name", menuName = "Weapon Template")]
public class WeaponTemplate : ScriptableObject
{
    public WeaponStruct data = new()
    {
        weaponName = "Weapon_Name",
        caliber = "Caliber",
        
        burstSize = 1,
        isFullAuto = true,
        hasFireSelector = true,
        fireModeSwitchTime = 0.2f,
        
        damage = 50,
        bulletCount = 1,
        fireRate = 600f,
        magSize = 30,
        totalAmmo = 300,
        reloadTime = 2.5f,
        reloadTimePartial = 2.0f,
        weaponSwitchTime = 0.5f,
        spread = 1f,
        range = 100f,
        decay = 10,
        recoil = 1.5f,
        
        ammo = 0,
        fireTime = 0,
        bulletPrefab = null,
        
        special = WeaponStruct.Special.None,
        sInt = 0,
        sFloat = 0,
    };
}
