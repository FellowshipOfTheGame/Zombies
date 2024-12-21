using UnityEngine;

[System.Serializable]
public struct WeaponStruct
{
    public bool   isClosedBolt;
    public bool   isFullAuto;
    public string weaponName;
    public int    damage;
    public int    burstSize;
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
    // public float  recoil;
}

[CreateAssetMenu(fileName = "Weapon_name", menuName = "Weapon Template")]
public class WeaponTemplate : ScriptableObject
{
    public WeaponStruct data;

    private void OnValidate()
    {
        // Set default values
        data.isFullAuto = true;
        data.weaponName = "Weapon_Name";
        data.damage = 50;
        data.burstSize = 1;
        data.bulletCount = 1;
        data.fireRate = 600f;
        data.magSize = 30;
        data.ammo = 30;
        data.totalAmmo = 300;
        data.caliber = "Caliber";
        data.reloadTime = 2.5f;
        data.reloadTimePartial = 2.0f;
        data.switchTime = 0.2f;
        data.spread = 1f;
        data.range = 100f;
        data.decay = 10;
        // data.recoil = 1.5f
    }
}