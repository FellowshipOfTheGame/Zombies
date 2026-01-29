using TMPro;
using UnityEngine;

public class FloatingDamage : MonoBehaviour
{
    public Transform playerCam;
    private readonly Vector3 rotationOffset = new(0f, 180f, 0f);

    public void Initialize(Transform playerCamera, float distanceScaler, int damage, Color color)
    {
        playerCam = playerCamera;
        TextMeshProUGUI textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.text = damage.ToString();
        textMesh.color = color;
        LateUpdate();
        
        Vector3 impulse = Random.Range(1.5f, 3.5f) * transform.right + Random.Range(1.5f, 3.5f) * transform.up;
        textMesh.GetComponent<Rigidbody>().AddRelativeForce(impulse * Mathf.Log(distanceScaler + 2), ForceMode.Impulse);
        
        textMesh.fontSize += 7.5f * distanceScaler;
    }
    
    private void LateUpdate()
    {
        transform.LookAt(playerCam);
        transform.Rotate(rotationOffset);
    }
}
