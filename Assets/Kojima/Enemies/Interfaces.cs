using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interfaces : MonoBehaviour
{
    
    public interface IDamage
    {
        void TakeDamage(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor);
    }

    public interface IDamageSpecial
    {
        void TakeDamageSpecial(int damage, Vector3 hitPosition, Transform textRotateTarget, Color textColor,
            string special, int percentage);
    }
    
}
