using System;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        // Faz a verificacao se a mira do jogador está apontando para algum objeto interagivel
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        GameObject objectHit = hit.transform.gameObject;
        if (!objectHit.TryGetComponent<IInteractable>(out var interactable)) return;
        
        interactable.Hover();
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactable.Interact(player);
        }
    }

}
