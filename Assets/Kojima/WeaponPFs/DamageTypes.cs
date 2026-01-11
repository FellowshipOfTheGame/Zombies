using UnityEngine;
using static InterfacesMNG;


public class DamageTypes : MonoBehaviour
{
    protected delegate void DealDamage(ICombat cachedICombat, RaycastHit target, int damage);
    protected DealDamage dealDamage;
    protected Transform mainCamera;
    
    
    // sem pre-caching
    // protected void NormalDamage(RaycastHit target, int damage)
    // {
    //     print(target.collider.GetComponent<IGet>()?.GetHealth());
    //     target.collider.GetComponent<ICombat>()?.TakeDamage(damage, target.point, mainCamera, Color.white);
    // }
    
    
    protected void NormalDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        print(target.collider.GetComponent<IGet>()?.GetHealth()); // DEBUG
        cachedICombat?.TakeDamage(damage, target.point, mainCamera, Color.white);
    }
    
    protected void ExplosiveDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        const float radius = 7.5f;
        foreach (Collider col in Physics.OverlapSphere(target.point, radius))
        {
            col.GetComponent<ICombat>()?.TakeDamage(
                Mathf.RoundToInt(damage * 0.3f), col.transform.position, mainCamera, Color.red/2f + Color.yellow/2f);
        }
    }
    
    protected void LowHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        cachedICombat?.TakeDamage(
            Mathf.FloorToInt(0.5f * damage * (1f-target.collider.gameObject.GetComponent<IGet>().GetHealthRatio())),
            target.point, mainCamera, 0.3f*Color.white);
    }
    
    protected void HighHealthDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        print(target.collider.GetComponent<IGet>().GetHealthRatio());
        cachedICombat?.TakeDamage(
            Mathf.FloorToInt(0.4f * damage * target.collider.GetComponent<IGet>().GetHealthRatio()),
            target.point, mainCamera, 0.5f*Color.black);
        NormalDamage(cachedICombat, target, damage);
    }
    
    protected void EchoDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, damage);
        cachedICombat?.TakeDamage(
            Mathf.FloorToInt(0.3f * damage ),
            target.point, mainCamera, 0.85f*Color.green);
    }
    
    protected void BleedDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        NormalDamage(cachedICombat, target, Mathf.FloorToInt(0.5f*damage));
        cachedICombat?.StackBleed(
            Mathf.FloorToInt(5 + 0.1f*damage), // stack amount
            0.75f, mainCamera, 0.85f*Color.red);
    }
    
    protected void TrueDamage(ICombat cachedICombat, RaycastHit target, int damage)
    {
        cachedICombat?.TrueDamage(
            damage, target.point, mainCamera, Color.cyan);
    }
}
