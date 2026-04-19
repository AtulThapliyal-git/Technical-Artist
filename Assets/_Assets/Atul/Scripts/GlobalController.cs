using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GlobalBendController : MonoBehaviour
{
    [Header("Bend Controls")]
    [Range(-50f, 50f)] public float horizontalBend = 15f;
    [Range(-50f, 50f)] public float verticalDrop = 0f;
    [Range(0f, 100f)] public float flatDistance = 47f;

    [Header("Required References")]
    [Tooltip("Drag your Road material(s) here!")]
    public Material[] bendingMaterials;

    // Shader Property IDs
    private static readonly int CurveAmountID = Shader.PropertyToID("_GlobalCurveAmount");
    private static readonly int CurveOriginID = Shader.PropertyToID("_GlobalCurveOrigin");
    private static readonly int CurveDistanceID = Shader.PropertyToID("_GlobalCurveDistance");

    void Update()
    {
        ApplyBend();
    }

    void OnValidate()
    {
        ApplyBend();
    }

    private void ApplyBend()
    {
        Vector2 bendAmount = new Vector2(horizontalBend, verticalDrop);
        Vector3 originPos = Vector3.zero;

        if (Camera.main != null)
        {
            originPos = Camera.main.transform.position;
        }

        foreach (Material mat in bendingMaterials)
        {
            if (mat != null)
            {
                mat.SetVector(CurveAmountID, bendAmount);
                mat.SetVector(CurveOriginID, originPos);
                mat.SetFloat(CurveDistanceID, flatDistance);
            }
        }
    }
}   