using UnityEngine;

public static class InterfacesMNG
{
    public interface ICombat
    {
        void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor);
        void StackBleed(int bleedStacks, float decayTime, Transform textRotateTarget, Color textColor);
    }
    
    public interface IGet
    {
        int GetHealth();
        int GetMaxHealth();
        float GetHealthRatio();
        int GetStacks();
    }
    
}
