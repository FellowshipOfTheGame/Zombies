using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    protected PlayerStruct data;
    
    [Header("Camera")]
    private float yaw;
    private float pitch;
    private float sensitivity = 150f;
    private Transform mainCamera;
    
    [Header("Movement")]
    protected float maxSpeed;
    protected Rigidbody rb;
    private Vector3 moveInput;

    [Header("Jump")]
    protected bool isOnGround;
    protected bool isOnSurface;
    protected bool canJump;
    protected bool jumpRequest;
    protected Coroutine onAirC;
    

    protected virtual void Start()
    {
        data = GetComponent<PlayerStats>().playerSO.data;
        
        mainCamera = transform.GetChild(0).transform;

        rb = GetComponent<Rigidbody>();
        maxSpeed = data.speedRatio * rb.linearDamping / 5;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump) jumpRequest = true;
        
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; // mouseX / Y-axis rotation
        pitch += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // mouseY / X-axis rotation

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        mainCamera.localRotation = Quaternion.Euler(-pitch, 0f, 0f);

        Vector3 forward = transform.forward; // Z‑axis of the player
        Vector3 right = transform.right; // X‑axis of the player

        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        moveInput = (forward * moveInput.z + right * moveInput.x).normalized;
    }
    
    // handle ground jumping
    protected virtual void FixedUpdate()
    {
        // if (isOnSurface) 
        rb.AddForce(maxSpeed * moveInput, ForceMode.VelocityChange);
        
        if (!jumpRequest) return;
        
        if (isOnGround) // ground jump
        {
            jumpRequest = canJump = false;
            rb.linearDamping = data.groundDamping/5;
            rb.AddForce(data.jumpSpeed * Vector3.up, ForceMode.VelocityChange);
        }
    }

    protected IEnumerator Gravity()
    {
        while (!isOnGround || !isOnSurface)
        {
            yield return new WaitForFixedUpdate();
            rb.AddForce(10 * maxSpeed * Vector3.down, ForceMode.Acceleration);
        }
        onAirC = null;
    }
    
    protected virtual void OnCollisionEnter(Collision col)
    {
        // either on the ground, or on a surface
        if (col.gameObject.CompareTag("Ground"))
        {
            canJump = isOnGround = true;
            rb.linearDamping = data.groundDamping;
        }
        else
        {
            isOnSurface = true;
        }
        
        maxSpeed = data.speedRatio * rb.linearDamping / 5;
    }

    protected virtual void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground")) // if left the ground
        {
            isOnGround = canJump = false;
            if (!isOnSurface) Float(); // and if is not touching a wall, then float
        }
        
        else if (col.gameObject.CompareTag("Wall")) // if un-touched the wall
        {
            isOnSurface = false;
            if(!isOnGround) Float(); // and if is not on the ground, then float
        }
        
        maxSpeed = data.speedRatio * rb.linearDamping / 5;
    }

    private void Float()
    {
        // isOnGround = isOnSurface = false;
        rb.linearDamping = data.groundDamping/5;
        onAirC ??= StartCoroutine(Gravity());
    }
}