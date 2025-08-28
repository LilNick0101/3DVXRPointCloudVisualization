using UnityEngine;

public class ViewerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public Transform orientation;
    public Canvas pauseCanvas;

    float horizontalInput;
    float verticalInput;
    float goUp;
    float goDown;

    [Header("Keybinds")]
    public KeyCode upFlight = KeyCode.Space;
    public KeyCode downFlight = KeyCode.LeftShift;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate() {
        if (!pauseCanvas.enabled) { 
            MoveViewver();
        }
        ReduceSpeed();
    }

    private void ReduceSpeed() {
        Vector3 flatVelocity = new(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        flatVelocity *= 0.85f;

        rb.linearVelocity = flatVelocity;
    }

    private void Update() {
        MyInput();
        SpeedControl();
    }

    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        goUp = Input.GetKey(upFlight) ? 1f : 0f;
        goDown = Input.GetKey(downFlight) ? 1f : 0f;
    }

    private void MoveViewver() {

        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput + orientation.up * (goUp - goDown));

        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
        
    }

    private void SpeedControl() {
        Vector3 flatVelocity = new(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        if (flatVelocity.sqrMagnitude > moveSpeed * moveSpeed) { 
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new(limitedVelocity.x, limitedVelocity.y, limitedVelocity.z);
        }
    }
}
