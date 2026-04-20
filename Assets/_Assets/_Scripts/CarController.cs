using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private Vector2 minMaxXPos = new Vector2(5f, -5f);
    [SerializeField] private float horizontalStepCount = 5f;
    [SerializeField] private float swipeThreshold = 50f;

    [Header("Dashy Crashy Rotation")]
    [Tooltip("How much the car leans on two wheels (Z-axis)")]
    [SerializeField] private float leanMultiplier = 1.5f;
    [Tooltip("How much the car points its nose (Y-axis)")]
    [SerializeField] private float steerMultiplier = 1.2f;
    [SerializeField] private float maxLeanAngle = 35f;  // Increased to match the image's extreme lean
    [SerializeField] private float maxSteerAngle = 20f;
    [SerializeField] private float rotationSmoothness = 15f;

    private bool isMoving = false;
    private int currentStep = 0;
    private Vector3 targetPosition;
    
    // Swipe tracking
    private Vector2 swipeStartPosMouse;
    private bool isSwipingMouse = false;
    private Vector2 swipeStartPosTouch;
    private bool isSwipingTouch = false;

    // Velocity tracking for the "Arc" rotation
    private float lastXPosition;

    private void Start()
    {
        // Set initial positions so the car doesn't fly wildly on frame 1
        targetPosition = transform.position;
        lastXPosition = transform.position.x;
    }

    private void Update()
    {
        HandleMouseSwipe();
        HandleTouchSwipe();

        float currentX = transform.position.x;
        float newX = Mathf.Lerp(currentX, targetPosition.x, Time.deltaTime * speed);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        float xVelocity = 0f;
        if (Time.deltaTime > 0.0001f) 
        {
            xVelocity = (newX - lastXPosition) / Time.deltaTime;
        }
        lastXPosition = newX;
    
        float targetRoll = -xVelocity * leanMultiplier;
        float targetYaw = xVelocity * steerMultiplier;

        targetRoll = Mathf.Clamp(targetRoll, -maxLeanAngle, maxLeanAngle);
        targetYaw = Mathf.Clamp(targetYaw, -maxSteerAngle, maxSteerAngle);

        Quaternion targetRotation = Quaternion.Euler(0, targetYaw, targetRoll);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);

        isMoving = Mathf.Abs(targetPosition.x - transform.position.x) > 0.05f;
    }

    private void HandleMouseSwipe()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isSwipingMouse = true;
            swipeStartPosMouse = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isSwipingMouse)
        {
            Vector2 endPos = Mouse.current.position.ReadValue();
            Vector2 delta = endPos - swipeStartPosMouse;
            isSwipingMouse = false;

            if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                MoveHorizontal(delta.x < 0f ? 1 : -1);
            }
        }
    }

    private void HandleTouchSwipe()
    {
        if (Touchscreen.current == null) return;

        var primary = Touchscreen.current.primaryTouch;
        if (!primary.press.isPressed && !isSwipingTouch) return;

        if (primary.press.wasPressedThisFrame)
        {
            isSwipingTouch = true;
            swipeStartPosTouch = primary.position.ReadValue();
        }
        else if (primary.press.wasReleasedThisFrame && isSwipingTouch)
        {
            Vector2 endPos = primary.position.ReadValue();
            Vector2 delta = endPos - swipeStartPosTouch;
            isSwipingTouch = false;

            if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                MoveHorizontal(delta.x < 0f ? 1 : -1);
            }
        }
    }

    private void MoveHorizontal(int direction)
    {
        float stepSize = (minMaxXPos.y - minMaxXPos.x) / horizontalStepCount;
        currentStep += direction;
        currentStep = Mathf.Clamp(currentStep, 0, (int)horizontalStepCount);
        float newXPos = minMaxXPos.x + currentStep * stepSize;
        
        targetPosition = new Vector3(newXPos, transform.position.y, transform.position.z);
        isMoving = true;
    }

	// This detects when your player car hits a traffic car
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Traffic"))
        {
            Debug.Log("BOOM! CRASHED INTO TRAFFIC!");
            
            // Temporary crash effect: Stop the car from moving
            speed = 0f; 
            
            // You can add an explosion particle effect or Game Over screen here later!
        }
    }
}