using UnityEngine;

public class ArcadeCameraFollow : MonoBehaviour
{
    [Header("Target Tracking")]
    [Tooltip("Drag your Car GameObject here")]
    public Transform target;
    
    [Tooltip("The default distance/height from the car. Leave at (0,0,0) to auto-grab on Start.")]
    public Vector3 offset;

    [Header("Damping (Lower is tighter, Higher is slower/more lag)")]
    [Tooltip("How fast the camera catches up moving forward. Keep this low (e.g., 0.05)")]
    public float forwardSmoothTime = 0.05f;
    
    [Tooltip("How much lag on lane changes. Higher = more juicy Dashy Crashy feel! (e.g., 0.2)")]
    public float sideSmoothTime = 0.2f;

    [Header("Look Settings")]
    [Tooltip("Should the camera slightly tilt to look at the car?")]
    public bool lookAtTarget = true;
    public float lookSmoothSpeed = 10f;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;
    private float zVelocity = 0.0f;

    void Start()
    {
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float smoothX = Mathf.SmoothDamp(transform.position.x, desiredPosition.x, ref xVelocity, sideSmoothTime);
        
        float smoothY = Mathf.SmoothDamp(transform.position.y, desiredPosition.y, ref yVelocity, forwardSmoothTime);
        
        float smoothZ = Mathf.SmoothDamp(transform.position.z, desiredPosition.z, ref zVelocity, forwardSmoothTime);

        transform.position = new Vector3(smoothX, smoothY, smoothZ);

        if (lookAtTarget)
        {
            Vector3 lookDirection = target.position - transform.position;
            lookDirection.y += 2f; 
            
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSmoothSpeed * Time.deltaTime);
        }
    }
}