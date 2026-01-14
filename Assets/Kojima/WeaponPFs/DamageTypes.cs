using UnityEngine;
using static InterfacesMNG;


public class DamageTypes : MonoBehaviour
{
    protected delegate void DealDamage(ICombat cachedICombat, RaycastHit target, int damage);
    protected DealDamage dealDamage;
    protected Transform mainCamera;
    protected WeaponStruct data;
    
    [Header("Colors")] // dano no shield color*=0.5f
    private readonly Color  cNormal = new(1, 1, 1);
    private readonly Color cExplode = new(1.0f, 0.5f, 0.1f);
    private readonly Color    cLowH = new(0.1f, 1.0f, 1.0f);
    private readonly Color   cHighH = new(0.1f, 1.0f, 1.0f);
    private readonly Color    cEcho = new(1.0f, 1.0f, 0.1f);
    private readonly Color     cDoT = new(0.8f, 0.3f, 0.9f);
    private readonly Color  cPoison = new(0.1f, 1.0f, 0.1f);
    private readonly Color   cBleed = new(1.0f, 0.1f, 0.1f);
    private readonly Color    cTrue = new(0, 0, 0);
    
    
    // sem pre-caching
    // protected void NormalDamage(RaycastHit target, int damage)
    // {
    //     print(target.collider.GetComponent<IGet>()?.GetHealth());
    //     target.collider.GetComponent<ICombat>()?.TakeDamage(damage, target.point, mainCamera, Color.white);
    // }
    
    
    protected void NormalDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        print(target.collider.GetComponent<IGet>()?.GetHealth()); // DEBUG
        cachedICombat?.TakeDamage(damage, target.point, mainCamera, cNormal);
    }
    
    protected void ExplosiveDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        foreach (Collider col in Physics.OverlapSphere(target.point, data.sRange))
        {
            col.GetComponent<ICombat>()?.TakeDamage(
                Mathf.RoundToInt(damage * data.sRatio), col.transform.position, mainCamera, cExplode);
        }
    }
    
    protected void LowHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        if (target.collider.GetComponent<IGet>()?.GetHealth() != 0)
            cachedICombat?.TakeDamage(
                Mathf.FloorToInt(data.sRatio * damage * (1f-target.collider.gameObject.GetComponent<IGet>().GetHealthRatio())),
                target.point, mainCamera, cLowH);
    }
    
    protected void HighHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.TakeDamage(
            Mathf.FloorToInt(data.sRatio * damage * target.collider.GetComponent<IGet>().GetHealthRatio()),
            target.point, mainCamera, cHighH);
        NormalDamage(cachedICombat, target, damage);
    }
    
    protected void EchoDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        cachedICombat?.DelayedDamage( Mathf.FloorToInt(damage*data.sRatio),
            data.sTickTime, mainCamera, cEcho);
    }
    
    protected void DamageOverTime(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, Mathf.FloorToInt(damage*(1f-data.sRatio)));
        cachedICombat?.DamageOverTime(
            Mathf.CeilToInt(damage*data.sRatio),
            data.sCount, data.sTickTime, mainCamera, cDoT);
    }
    
    protected void PoisonDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, Mathf.FloorToInt((1f - data.sRatio) * damage));
        cachedICombat?.PoisonDamage(
            Mathf.FloorToInt(data.sRatio * damage), // stack amount
            data.sTickTime, mainCamera, cPoison);
    }
    
    protected void BleedDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, Mathf.FloorToInt(damage*(1f-data.sRatio)));
        cachedICombat?.BleedDamage(
            Mathf.CeilToInt(damage*data.sRatio), data.sTickTime, mainCamera, cBleed);
    }
    
    protected void TrueDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.TrueDamage(damage, target.point, mainCamera, cTrue);
    }
}
