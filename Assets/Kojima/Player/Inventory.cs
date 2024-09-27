using System.Collections.Generic;
using UnityEngine;
namespace WeaponsNS
{
    public interface IWeaponDataProvider
    {
        WeaponInfoStruct GetWeaponData();
    }
    
    public struct WeaponInfoStruct
    {
        public bool   isClosedBolt; //partial reload ou re-chamber
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
        //int recuo; //forca em newtons pra arma dar coice pra cima quando atirar(?)
    }
    
    public class Inventory : MonoBehaviour
    {
        private readonly Dictionary<int, WeaponInfoStruct> inventoryDict = new();
        public Dictionary<int, WeaponInfoStruct> InventoryDictReference => inventoryDict; //link for other scripts to use this dictionary
    }
    
}
