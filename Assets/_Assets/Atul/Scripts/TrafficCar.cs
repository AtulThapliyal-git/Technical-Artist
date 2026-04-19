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

        transform.Translate(Vector3.forward * backwardSpeed * Time.deltaTime);

        if (transform.position.z < despawnZPosition)
        {
            Destroy(gameObject);
        }
    }
}