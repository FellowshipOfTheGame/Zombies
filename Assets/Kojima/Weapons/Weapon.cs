using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    public void Hover()
    {
    }
    
    public void Interact()
    {
        Debug.Log("interact");
    }
}
