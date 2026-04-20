using UnityEngine;

public class SpawnerLoop : MonoBehaviour
{
    [SerializeField] private Transform[] movingBoddies;
    [Tooltip("Where the road spawns in the distance")]
    [SerializeField] private float startZ = 50f;
    
    [Tooltip("Where the road despawns behind the camera. (Make sure this is negative!)")]
    [SerializeField] private float offCameraZ = -50f; 
    
    [SerializeField] private float segmentLength = 10f;

    private void Update()
    {
        LoopBodies();
    }

    private void LoopBodies()
    {
        if (movingBoddies == null || movingBoddies.Length == 0)
        {
            return;
        }
        
        foreach (Transform body in movingBoddies)
        {
            // Assuming the pieces are moving towards the camera (negative Z)
            if (body.position.z <= offCameraZ)
            {
                // 1. Calculate exactly how far past the despawn line it went this frame.
                // If offCameraZ is -50, and it reached -50.4, the overshoot is -0.4.
                float overshoot = body.position.z - offCameraZ;

                // 2. Add that exact overshoot to the StartZ.
                // So instead of spawning at exactly 50, it spawns at 49.6, perfectly closing the gap!
                float perfectSnapZ = startZ + overshoot;

                body.position = new Vector3(body.position.x, body.position.y, perfectSnapZ);
            }
        }
    }
}