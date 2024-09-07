using UnityEngine;
[CreateAssetMenu(fileName = "Weapon_name", menuName = "Weapon Template")]

public class WeaponTemplate : ScriptableObject
{
    public bool   isClosedBolt = true;
    public bool   isFullAuto   = true;
    public string weaponName   = "Weapon_Name";
    public int    damage       = 50;
    public int    burstSize    = 1;
    public int    bulletCount  = 1;
    public float  fireRate     = 600f;
    public int    magCapacity  = 30;
    public int    ammo         = 30;
    public int    totalAmmo    = 300;
    public string caliber      = "Caliber";
    public float  reloadTime   = 2.5f;
    public float  reloadTimePartial = 2.0f;
    public float  switchTime   = 0.2f;
    public float  spread       = 1f;
    public float  range        = 100f; //effective range, maxRange=4*range
    public int    decay        = 10;
    //public int recuo; //forca em newtons pra arma dar coice pra cima quando atirar
}
