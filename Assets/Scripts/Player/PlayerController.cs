using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Grab the "Move" action from the "Player" action map
        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        // Read input every frame, but apply movement in FixedUpdate
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // MovePosition respects physics collisions, unlike directly setting transform.position
        Vector2 newPosition = rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}