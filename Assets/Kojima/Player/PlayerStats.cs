using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerTemplate playerSO;

    private void Start()
    {
        if (playerSO.data.canWallJump) gameObject.AddComponent<WallJump>();
        else gameObject.AddComponent<PlayerMovement>();
        // if (playerSO.data.canDoubleJump) gameObject.AddComponent<DoubleJump>();
    }
}
