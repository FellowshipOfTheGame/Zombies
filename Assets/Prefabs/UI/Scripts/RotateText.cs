using UnityEngine;

public class RotateText : MonoBehaviour
{
    public Transform textRotateTarget;
    
    void Update()
    {
        transform.rotation = textRotateTarget.transform.rotation;
    }
}
