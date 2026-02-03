using UnityEngine;

public class WallJump : PlayerMovement
{
    private bool isGrounded;
    [SerializeField] private float wallDamping = 1f;

    protected override void FixedUpdate()
    {
        if (isGrounded) rb.AddForce(maxSeed * moveInput, ForceMode.VelocityChange);
        // else pra air strafing?
        
        if ( !(jumpRequest && canJump) ) return;
        
        // else set up the jump and verify where the player jumped from
        canJump = false;
        jumpRequest = false;
        rb.linearDamping = 0f;
        
        if (isGrounded) JumpFromGround();
        else JumpFromWall();
    }

    private void JumpFromGround()
    {
        rb.AddForce(data.jumpSpeed * Vector3.up, ForceMode.VelocityChange);
    }

    private void JumpFromWall()
    {
        // calculate the normal on the jump input
        var normal = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
        // Debug.DrawLine(transform.position, transform.position + normal, Color.orangeRed, 2.5f);
        
        bool hit = Physics.Raycast(transform.position, normal, 0.7f);
        normal = (new Vector3(0, 0.75f, 0) + (hit ? -normal : normal)).normalized;
        rb.AddForce(data.jumpSpeed/1.15f * normal, ForceMode.VelocityChange);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        string cTag = collision.gameObject.tag;
        switch (cTag)
        {
            case "Ground":
                isGrounded = true;
                canJump = true;
                rb.linearDamping = data.groundDamping;
                break;
            case "Wall" when !isGrounded:
                canJump = true;
                rb.linearDamping = wallDamping;
                break;
        }
        
        maxSeed = data.speedRatio * rb.linearDamping / 5f;
    }

    protected override void OnCollisionExit(Collision collision)
    {
        if (!isGrounded) rb.linearDamping = 0f;
        
        // if the player forcefully left the ground/wall, they should not be able to jump until ground contact
        string cTag = collision.gameObject.tag;
        switch (cTag)
        {
            case "Ground":
                isGrounded = false;
                canJump = false;
                break;
            case "Wall" when !isGrounded:
                canJump = false;
                break;
        }
    }
}
