using UnityEngine;

public static class InterfacesMNG
{
    public interface IDmg
    {
        void TakeDmg(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor);
    }
    
    public interface IDmgSpecial
    {
        void TakeDmgSpecial(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor,
            string special, int percentage);
    }
}
