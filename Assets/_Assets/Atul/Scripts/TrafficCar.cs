using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    [Header("Endless Runner Speed")]
    [Tooltip("MUST be a negative number! Make it closer to 0 than your road speed to overtake it.")]
    public float backwardSpeed = -30f; 
    
    [Tooltip("How far behind the player before the car deletes itself.")]
    public float despawnZPosition = -20f;

    void Update()
    {
        // Move the car backward along the Z axis toward the player
        transform.Translate(Vector3.forward * backwardSpeed * Time.deltaTime);

        // Delete the car once you pass it so the game doesn't crash from too many objects
        if (transform.position.z < despawnZPosition)
        {
            Destroy(gameObject);
        }
    }
}