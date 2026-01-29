using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool canJump = true;
    private float sensitivity = 150f;
    private float maxSpeed = 1f;
    private float pitch;
    private float yaw;
    
    private Rigidbody rb;
    private Vector3 moveInput;
    private Transform mainCamera;

    private void Start()
    {
        mainCamera = transform.GetChild(0).transform;
        
        rb = GetComponent<Rigidbody>();
        maxSpeed *= rb.linearDamping/5f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; // mouseX / Y-axis rotation
        pitch += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // mouseY / X-axis rotation
        
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        mainCamera.localRotation = Quaternion.Euler(-pitch, 0f, 0f);
        
        Vector3 forward = transform.forward;   // Z‑axis of the player
        Vector3 right   = transform.right;     // X‑axis of the player
        
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        moveInput = ((forward * moveInput.z) + (right * moveInput.x)).normalized;
    }

    private void FixedUpdate()
    {
        if (canJump) rb.AddForce(maxSpeed * moveInput, ForceMode.VelocityChange);
        
        if (canJump && Input.GetKey(KeyCode.Space))
        {
            canJump = false;
            rb.linearDamping = 0f;
            rb.AddForce(10f * rb.mass * Vector3.up, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            rb.linearDamping = 10f;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = false;
            rb.linearDamping = 0f;
        }
    }
}