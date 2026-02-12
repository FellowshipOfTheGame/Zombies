using UnityEngine;

public class WallJump : PlayerMovement
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // applies player movement and reads for a jump
        if (!jumpRequest) return;
        
        if (isOnSurface) // wall jump
        {
            isOnSurface = canJump = jumpRequest = false;
            rb.linearDamping = data.groundDamping/5;
            
            // calculate the normal on the jump input
            var normal = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
            // line debug tool to render the normal
            // Debug.DrawLine(transform.position, transform.position + normal, Color.orangeRed, 2.5f);
            
            bool hit = Physics.Raycast(transform.position, normal, 0.7f); // check to which side is the wall
            normal = (new Vector3(0, 0.35f, 0) + 1.5f * (hit ? -normal : normal)).normalized; // jump up and away from the wall
            rb.AddForce(data.jumpSpeed * normal, ForceMode.VelocityChange);
            
            onAirC ??= StartCoroutine(Gravity());
        }
    }
    
    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col); // set the collision flags
        
        if (isOnSurface && !isOnGround) canJump = true; // if floating and touch a wall
    }

    protected override void OnCollisionExit(Collision col)
    {
        base.OnCollisionExit(col);
        
        if (isOnSurface && !isOnGround) canJump = true; // if touching a wall and jump
        
        // if didn't use the wall jump when un-touching a wall, then lose it
        if (canJump && !isOnGround && col.gameObject.CompareTag("Wall")) canJump = false;
    }
}
