using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private Transform playerCamera;

    private CharacterController characterController;
    private PlayerHUD playerHUD;
    private float verticalVelocity;
    private const float gravity = -9.81f;
    private float xRotation;

    private float currentSpeed;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerHUD = GetComponent<PlayerHUD>();
        
        // Esconde o cursor do mouse e trava ele no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Rotate();
        Jump();
        ApplyGravity();
    }

    // retorna true se o jogador estiver no chao
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // Aplica a gravidade ao jogador
    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }
        }

        Vector3 gravityMove = new(0, verticalVelocity, 0);
        characterController.Move(gravityMove * Time.deltaTime);
    }

    // Move o jogador
    private void Move()
    {
        Vector3 move = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // Transforma o vetor de movimento considerando a direcao do jogador
        move = transform.TransformDirection(move);

        // Checa se o jogador esta andando ou correndo
        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = running ? runSpeed : walkSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift)) { playerHUD.HideCrosshair(); }
        if (Input.GetKeyUp(KeyCode.LeftShift))   { playerHUD.ShowCrosshair(); }

        if (move.magnitude > 0)
        {
            // Acelera mais rapido se o jogador estiver correndo
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 
                (running ? acceleration * walkSpeed : acceleration * runSpeed) * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, 
                acceleration * walkSpeed * Time.deltaTime); // Desacelera
        }

        characterController.Move(currentSpeed * Time.deltaTime * move);
    }

    // Faz o jogador pular
    private void Jump()
    {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 gravityMove = new(0, verticalVelocity, 0);
        characterController.Move(gravityMove * Time.deltaTime);
    }

    // Rotaciona o jogador e a camera
    private void Rotate()
    {
        // Input do mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotaciona o jogador horizontalmente (o modelo n gira pq o characterController trava a rotacao)
        transform.Rotate(Vector3.up * mouseX);

        // Rotaciona a camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Previne que o jogador vire a cabeca pra tras
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
