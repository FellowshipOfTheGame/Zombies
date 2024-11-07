using System;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject objectHit = hit.transform.gameObject;
            if (objectHit.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
    }

}
