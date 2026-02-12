using UnityEngine;

public class DoubleJump : PlayerMovement
{
    private bool canDoubleJump = true;
    private bool doubleJumpRequest;

    private new void Update()
    {
        if (canJump || !canDoubleJump) return; // if already jumped and can still doubleJump, continue
        if (Input.GetKeyDown(KeyCode.Space)) doubleJumpRequest = true;
    }
    
    private new void FixedUpdate()
    {
        if (doubleJumpRequest)
        {
            doubleJumpRequest = false;
            canDoubleJump = false;
            rb.AddForce(data.jumpSpeed * Vector3.up, ForceMode.VelocityChange);
        }
    }

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if (col.gameObject.CompareTag("Ground")) canDoubleJump = true;
    }
}
