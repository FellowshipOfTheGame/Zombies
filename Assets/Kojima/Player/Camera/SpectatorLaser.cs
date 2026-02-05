using UnityEngine;

public class SpectatorLaser : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private float maxDistance = 500f;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Vector3 dir = transform.forward;
        Vector3 origin = transform.position;

        lr.startColor = lr.endColor = rayColor;
        lr.SetPosition(0, origin);
        
        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            lr.SetPosition(1, hit.point);
        else lr.SetPosition(1, origin + dir * maxDistance);
    }
}
