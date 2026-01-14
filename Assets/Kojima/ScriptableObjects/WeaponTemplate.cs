using UnityEngine;

[System.Serializable]
public struct WeaponStruct
{
    public bool   hasFireSelector;
    public bool   isFullAuto;
    public string weaponName;
    public int    damage;
    public int    burstSize; // se for 1, desliga a opcao de burst
    public int    bulletCount;
    public float  fireRate;
    public float  fireTime;
    public int    magSize;
    public int    ammo;
    public int    totalAmmo;
    public string caliber;
    public float  reloadTime;
    public float  reloadTimePartial;
    public float  switchTime;
    public float  spread;
    public float  range;
    public int    decay;
    public float  recoil;
    public enum Special
    {
        None,
        Explosive,  // damage + sRatio*damage with sRange
        LowHealth,  // damage + up to sRatio*damage
        HighHealth, // damage + up to sRatio*damage
        Echo,       // damage + sRatio*damage with sTickTime delay
        True,       // damage
        Poison,     // damage*(1-sRatio) + damage*sRatio stacks with sTickTime
        Bleed,      // damage*(1-sRatio) + damage*sRatio dot with sTickTime
        DoT,        // damage*(1-sRatio) + damage*sRatio dot sCount times with sTickTime
    }
    public Special special;
    public float sRange;
    public float sRatio;
    public float sTickTime;
    public int sCount;
}

[CreateAssetMenu(fileName = "Weapon_name", menuName = "Weapon Template")]
public class WeaponTemplate : ScriptableObject
{
    public WeaponStruct data = new()
    {
        hasFireSelector = true,
        isFullAuto = true,
        weaponName = "Weapon_Name",
        damage = 50,
        burstSize = 1,
        bulletCount = 1,
        fireRate = 600f,
        magSize = 30,
        ammo = 30,
        totalAmmo = 300,
        caliber = "Caliber",
        reloadTime = 2.5f,
        reloadTimePartial = 2.0f,
        switchTime = 0.5f,
        spread = 1f,
        range = 100f,
        decay = 10,
        recoil = 1.5f
    };
}
