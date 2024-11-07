using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Variables")]
    public float maxSpeed = 10f;
    public float jumpSpeed = 6f;
    private float fallSpeed, hInput, vInput;
    
    [Header("Components")]
    private Rigidbody rb;  // this.rigidBody
    private Vector3 moveDirection, currentSpeed;
    public Transform playerTransform;
    
    [Header("Ground check")]
    public LayerMask groundMask;
    public bool isGrounded;
    
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    
    private void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");  //raycast pra ver se encosta no chao \ maxDistance=altura/2
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.05f, groundMask);
        
        //input.normalize \ salva a V(y) \ "acelera" o player limita a velocidade pra maxSpeed \ restaura o V(y)
        moveDirection = (playerTransform.right * hInput + playerTransform.forward * vInput).normalized;
        fallSpeed = rb.linearVelocity.y;
        currentSpeed = rb.linearVelocity;
        currentSpeed.y = 0;
        currentSpeed *= 0.75f; //deixa o movimento mais controlavel
        currentSpeed += moveDirection * maxSpeed /5;
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, maxSpeed);
        
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            currentSpeed.y = jumpSpeed;
            rb.linearVelocity = currentSpeed;
        }
        else
        {
            currentSpeed.y = fallSpeed;
            rb.linearVelocity = currentSpeed;
        }
    }
}
