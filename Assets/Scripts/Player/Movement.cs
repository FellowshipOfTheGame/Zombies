using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody rb;
    private Vector3 moveDirection, currentSpeed;
    public Transform playerTransform;
    private const float maxSpeed = 10f;
    private const float jumpForce = 4f;
    private float fallSpeed, hinput, vinput;

    [Header("Ground check")]
    public LayerMask groundMask;
    public bool isGrounded;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    
    void Update()
    {
        hinput = Input.GetAxis("Horizontal");
        vinput = Input.GetAxis("Vertical");  //raycast pra ver se encosta no chao \ maxDistance=altura/2
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.05f, groundMask);
        
        //input.normalize \ salva a V(y) \ "acelera" o player limita a velocidade pra maxSpeed \ restaura o V(y)
        moveDirection = (playerTransform.right * hinput + playerTransform.forward * vinput).normalized;
        fallSpeed = rb.velocity.y;
        currentSpeed = rb.velocity;
        currentSpeed.y = 0;
        currentSpeed *= 0.75f; //deixa o movimento mais controlavel
        currentSpeed += moveDirection * maxSpeed /5;
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, maxSpeed);
        
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            currentSpeed.y = 6f;
            rb.velocity = currentSpeed;
        }
        else
        {
            currentSpeed.y = fallSpeed;
            rb.velocity = currentSpeed;
        }
    }
}
