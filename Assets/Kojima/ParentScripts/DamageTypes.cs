using UnityEngine;
using static InterfacesMNG;
using static WeaponStruct;


public class DamageTypes : MonoBehaviour
{
    protected delegate void SpecialDamage(ICombat cachedICombat, RaycastHit target);
    protected SpecialDamage specialDamage;
    protected delegate void NormalDelegate(ICombat cachedICombat, RaycastHit target);
    protected NormalDelegate normalDelegate;
    
    protected LayerMask hitMask;
    protected ICombat playerICombat;
    protected Transform playerCamera;
    protected WeaponStruct data;

    
    [Header("Colors")] // dano no shield color*=0.5f
    protected readonly Color  cNormal = new(1, 1, 1);
    protected readonly Color cExplode = new(1.0f, 0.5f, 0.1f);
    protected readonly Color    cLowH = new(0.1f, 1.0f, 1.0f);
    protected readonly Color   cHighH = new(0.1f, 1.0f, 1.0f);
    protected readonly Color    cEcho = new(1.0f, 1.0f, 0.1f);
    protected readonly Color     cDoT = new(0.8f, 0.3f, 0.9f);
    protected readonly Color  cPoison = new(0.1f, 1.0f, 0.1f);
    protected readonly Color   cBleed = new(1.0f, 0.1f, 0.1f);
    protected readonly Color  cBleed2 = new(.69f, 0.0f, 0.0f);
    protected readonly Color    cTrue = new(0, 0, 0);

    protected void Awake()
    {
        hitMask = LayerMask.GetMask("Default", "Enemy");
    }

    protected virtual void OnEnable()
    {
        if (data.damage > 0) normalDelegate = NormalDamage;
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

    protected void DealDamage(ICombat cachedICombat, RaycastHit target)
    {
        normalDelegate?.Invoke(cachedICombat, target);
        specialDamage?.Invoke(cachedICombat, target);
    }
    
    protected void NormalDamage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.TakeDamage(data.damage, target.point, playerCamera, cNormal);
    }
    
    protected virtual void ExplosiveDamage(ICombat cachedICombat, RaycastHit target)
    {
        Collider[] colliders = Physics.OverlapSphere(target.point, data.sFloat);
        foreach (Collider col in colliders) DamageCollider(col, target.point, data.sInt);
    }
    
    protected void DamageCollider(Collider col, Vector3 point, int damage)
    {
        Vector3 direction = col.transform.position - point;
        float fallOff = direction.magnitude/data.sFloat; // decai linearmente
        // float falloff = Mathf.Pow(1f - (direction.magnitude / range), 2f); // decai exponencialmente
        
        damage = Mathf.FloorToInt(damage * fallOff);
        col.GetComponent<ICombat>()?.TakeDamage(damage, col.transform.position, col.transform, cExplode);
        col.attachedRigidbody?.AddForce(5f * fallOff * direction,  ForceMode.Impulse);
    }
    
    protected void LowHealthDamage(ICombat cachedICombat, RaycastHit target)
    {
        float healthRatio = target.collider.GetComponent<IGet>().GetHealthRatio();
        if ( healthRatio > 0)
            cachedICombat?.TakeDamage(
                Mathf.CeilToInt(data.sFloat/100f * data.damage * (1f - healthRatio)),
                target.point, playerCamera, cLowH);
    }
    
    protected void HighHealthDamage(ICombat cachedICombat, RaycastHit target)
    {
        IGet iGet = target.collider.GetComponent<IGet>();
        if ( iGet.GetHealth() > 0)
            cachedICombat?.TakeDamage(
                Mathf.CeilToInt(data.damage * data.sFloat/100 * 
                                 Mathf.Clamp01((iGet.GetHealth()+data.damage)/(float)iGet.GetMaxHealth())),
                target.point, playerCamera, cHighH);
    }
    
    protected void EchoDamage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.DelayedDamage( data.sInt, data.sFloat, playerCamera, cEcho);
    }
    
    protected void DamageOverTime(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.DamageOverTime(data.sInt, data.sFloat, playerCamera, cDoT);
    }
    
    protected void PoisonDamage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.PoisonDamage(data.sInt, data.sFloat, playerCamera, cPoison);
    }
    
    protected void BleedDamage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.BleedDamage(data.sInt, data.sFloat, playerCamera, cBleed);
    }

    protected void Hemorrhage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.Hemorrhage(data.sInt, data.sFloat, playerCamera, cBleed2);
    }
    
    protected void TrueDamage(ICombat cachedICombat, RaycastHit target)
    {
        cachedICombat?.TrueDamage(
            data.sInt - Mathf.FloorToInt(data.sInt * target.distance / (3 * data.range)),
            target.point, playerCamera, cTrue);
    }

    protected void Healing(ICombat cachedICombat, RaycastHit target)
    {
        // heal the player and not the target
        playerICombat.AddHealth(data.sInt);
    }

    protected void HealingOverTime(ICombat cachedICombat, RaycastHit target)
    {
        // heal the player and not the target
        playerICombat.HealingOverTime(data.sInt, data.sFloat, playerCamera, cBleed2);
    }
}
