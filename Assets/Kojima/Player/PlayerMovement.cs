using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    private float yaw;
    private float pitch;
    private float sensitivity = 150f;
    private Transform mainCamera;
    
    [Header("Movement")]
    protected float maxSeed;
    protected Vector3 moveInput;
    protected Rigidbody rb;
    protected readonly float speedRatio = 1f;
    protected readonly float groundDamping = 10f;
    
    [Header("Jump")]
    protected bool canJump;
    protected bool jumpRequest;
    protected float jumpSpeed = 10f;


    protected virtual void Start()
    {
        mainCamera = transform.GetChild(0).transform;

        rb = GetComponent<Rigidbody>();
        maxSeed = speedRatio * rb.linearDamping / 5f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
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
        moveInput = ((forward * moveInput.z) + (right * moveInput.x)).normalized;
    }

    protected virtual void FixedUpdate()
    {
        if (canJump) rb.AddForce(maxSeed * moveInput, ForceMode.VelocityChange);
        // else adiciona uma forca mais fraca pra air strafing

        if (canJump && jumpRequest)
        {
            jumpRequest = false;
            canJump = false;
            rb.linearDamping = 0f;
            rb.AddForce(jumpSpeed * Vector3.up, ForceMode.VelocityChange);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            rb.linearDamping = groundDamping;
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
            rb.linearDamping = 0f;
        }
    }
}