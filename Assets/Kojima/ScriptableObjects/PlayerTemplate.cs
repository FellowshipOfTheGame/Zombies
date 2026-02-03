using UnityEngine;

[System.Serializable]
public struct PlayerStruct
{
    [Header("Skills")]
    public bool canWallJump;
    public bool canDoubleJump;
    
    [Header("Stats")]
    public int maxHealth;
    public float jumpSpeed;
    public float groundDamping;
    [Tooltip("Max speed multiplier")] public float speedRatio;
    
    [Header("Health Regen")]
    [Tooltip("Delay to start regen")] public float regenDelay;
    [Tooltip("Initial regen value")]  public float regenStart;
    [Tooltip("Max regen speed")]      public float regenMax;
    [Tooltip("Logarithmic increase per tick")] public float regenIncrease;
}

[CreateAssetMenu(fileName = "Player_Stats", menuName = "Player Template")]
public class PlayerTemplate : ScriptableObject
{
    public PlayerStruct data = new()
    {
        canWallJump = false,
        canDoubleJump = false,
        
        maxHealth = 200,
        jumpSpeed = 10f,
        groundDamping = 10f,
        speedRatio = 1.0f,
        
        regenDelay = 4f,
        regenStart = 1f,
        regenMax = 10f,
        regenIncrease = 0.25f,
    };
}
