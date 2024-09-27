using UnityEngine;

public class RotateText : MonoBehaviour
{
    public Transform textRotateTarget;

    private void Update()
    {
        transform.rotation = textRotateTarget.transform.rotation;
    }
}
