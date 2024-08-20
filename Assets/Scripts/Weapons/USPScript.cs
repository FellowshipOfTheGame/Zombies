using UnityEngine;
namespace WeaponsNS
{
    public class USPScript : MonoBehaviour, IWeaponDataProvider
    {
        public WeaponTemplate template; // add reference in Unity's spector
        public WeaponInfoStruct GetWeaponData()
        {
            var data = new WeaponInfoStruct
            {
                isClosedBolt = template.isClosedBolt,
                isFullAuto   = template.isFullAuto,
                weaponName   = template.weaponName,
                damage       = template.damage,
                burstSize    = template.burstSize,
                fireRate     = template.fireRate,
                magSize      = template.magCapacity,
                ammo         = template.ammo,
                totalAmmo    = template.totalAmmo,
                caliber      = template.caliber,
                reloadTime   = template.reloadTime,
                reloadTimePartial = template.reloadTimePartial,
                switchTime   = template.switchTime,
                spread       = template.spread,
                range        = template.range,
                decay        = template.decay
            };
            return data;
        }
    }
}