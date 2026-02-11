using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // get the player stats and selects/initializes the scripts
    public PlayerTemplate playerSO;

    private void Start()
    {
        if (playerSO.data.canWallJump) gameObject.AddComponent<WallJump>();
        else gameObject.AddComponent<PlayerMovement>();
        // if (playerSO.data.canDoubleJump) gameObject.AddComponent<DoubleJump>();
    }
}
