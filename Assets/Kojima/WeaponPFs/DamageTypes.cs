using System;
using UnityEngine;
using static InterfacesMNG;
using static WeaponStruct;


public class DamageTypes : MonoBehaviour
{
    protected delegate void SpecialDamage(ICombat cachedICombat, RaycastHit target, int damage);
    protected SpecialDamage specialDamage;
    protected Transform mainCamera;
    protected WeaponStruct data;
    protected ICombat playerICombat;
    
    [Header("Colors")] // dano no shield color*=0.5f
    private readonly Color  cNormal = new(1, 1, 1);
    private readonly Color cExplode = new(1.0f, 0.5f, 0.1f);
    private readonly Color    cLowH = new(0.1f, 1.0f, 1.0f);
    private readonly Color   cHighH = new(0.1f, 1.0f, 1.0f);
    private readonly Color    cEcho = new(1.0f, 1.0f, 0.1f);
    private readonly Color     cDoT = new(0.8f, 0.3f, 0.9f);
    private readonly Color  cPoison = new(0.1f, 1.0f, 0.1f);
    private readonly Color   cBleed = new(1.0f, 0.1f, 0.1f);
    private readonly Color  cBleed2 = new(.69f, 0.0f, 0.0f);
    private readonly Color    cTrue = new(0, 0, 0);

    protected void OnEnable()
    {
        specialDamage = data.special switch
        {
            Special.Explosive  => ExplosiveDamage,
            Special.LowHealth  => LowHealthDamage,
            Special.HighHealth => HighHealthDamage,
            Special.Echo       => EchoDamage,
            Special.DoT        => DamageOverTime,
            Special.Poison     => PoisonDamage,
            Special.Bleed      => BleedDamage,
            Special.Hemorrhage => Hemorrhage,
            Special.True       => TrueDamage,
            Special.Healing    => Healing,
            Special.HoT        => HealingOverTime,
            _                  => null
        };
    }


    // sem pre-caching
    // protected void NormalDamage(RaycastHit target, int damage)
    // {
    //     print(target.collider.GetComponent<IGet>()?.GetHealth());
    //     target.collider.GetComponent<ICombat>()?.TakeDamage(damage, target.point, mainCamera, Color.white);
    // }

    protected void DealDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        if (damage > 0) NormalDamage(cachedICombat, target, damage);
        specialDamage?.Invoke(cachedICombat, target, damage);
    }
    
    protected void NormalDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.TakeDamage(damage, target.point, mainCamera, cNormal);
    }
    
    protected void ExplosiveDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        foreach (Collider col in Physics.OverlapSphere(target.point, data.sFloat))
        {
            col.GetComponent<ICombat>()?.TakeDamage(data.sInt, col.transform.position, mainCamera, cExplode);
        }
    }
    
    protected void LowHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        float healthRatio = target.collider.GetComponent<IGet>().GetHealthRatio();
        if ( healthRatio > 0)
            cachedICombat?.TakeDamage(
                Mathf.FloorToInt(data.sFloat/100f * damage * (1f - healthRatio)),
                target.point, mainCamera, cLowH);
    }
    
    protected void HighHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        IGet iGet = target.collider.GetComponent<IGet>();
        if ( iGet.GetHealth() > 0)
            cachedICombat?.TakeDamage(
                Mathf.FloorToInt(damage*data.sFloat/100 * ((iGet.GetHealth()+damage)/(float)iGet.GetMaxHealth())),
                target.point, mainCamera, cHighH);
    }
    
    protected void EchoDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.DelayedDamage( data.sInt, data.sFloat, mainCamera, cEcho);
    }
    
    protected void DamageOverTime(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.DamageOverTime(data.sInt, data.sFloat, mainCamera, cDoT);
    }
    
    protected void PoisonDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.PoisonDamage(data.sInt, data.sFloat, mainCamera, cPoison);
    }
    
    protected void BleedDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.BleedDamage(data.sInt, data.sFloat, mainCamera, cBleed);
    }

    protected void Hemorrhage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.Hemorrhage(data.sInt, data.sFloat, mainCamera, cBleed2);
    }
    
    protected void TrueDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.TrueDamage(
            data.sInt - Mathf.FloorToInt(data.sInt * target.distance / (3 * data.range)),
            target.point, mainCamera, cTrue);
    }

    protected void Healing(ICombat cachedICombat, RaycastHit target, int damage)
    {
        // heal the player and not the target
        playerICombat.AddHealth(data.sInt);
    }

    protected void HealingOverTime(ICombat cachedICombat, RaycastHit target, int damage)
    {
        // heal the player and not the target
        playerICombat.HealingOverTime(data.sInt, data.sFloat, mainCamera, cBleed2);
    }
}
