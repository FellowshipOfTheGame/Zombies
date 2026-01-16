using UnityEngine;

public static class InterfacesMNG
{
    public interface ICombat
    {
        void AddHealth(int health);
        void AddShield(int shield);
        void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor);
        void TrueDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor);
        void DelayedDamage(int damage, float tickTime, Transform textRotateTarget, Color textColor);
        
        // DoT com base em acumulo, decaimento linear
        void DamageOverTime(int dps, float duration, Transform textRotateTarget, Color textColor);
        
        // DoT com base em stacks, decaimento geometrico/exponencial
        void PoisonDamage(int poisonStacks, float halfLife, Transform textRotateTarget, Color textColor);
        
        // DoT com base em acumulo, decaimento geometrico/exponencial
        void BleedDamage(int bleedDamage, float tickTime, Transform textRotateTarget, Color textColor);
    }
    
    public interface IGet
    {
        int GetHealth();
        int GetMaxHealth();
        float GetHealthRatio();
        int GetStacks();
    }
    
}
