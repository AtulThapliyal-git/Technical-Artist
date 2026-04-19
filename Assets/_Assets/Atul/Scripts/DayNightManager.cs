using UnityEngine;

public class CustomDayNightManager : MonoBehaviour
{
    [Header("Time Control")]
    public float fullCycleDuration = 60f; 
    [Range(0f, 1f)] public float timeOfDay = 0.15f;

    [Header("Sky Planes (Static)")]
    public MeshRenderer dawnSky;
    public MeshRenderer daySky;
    public MeshRenderer sunsetSky;
    public MeshRenderer nightSky;

    [Header("Cloud Planes (Scrolling)")]
    public MeshRenderer dawnClouds;
    public MeshRenderer dayClouds;
    public MeshRenderer sunsetClouds;
    public MeshRenderer nightClouds;

    [Header("Cloud Scrolling Speed")]
    public Vector2 cloudScrollSpeed = new Vector2(0.02f, 0f);
    private Vector2 currentCloudOffset;

    [Header("Material Swapping (Ground)")]
    public MeshRenderer[] objectsToChange;
    public Material dawnMaterial;
    public Material dayMaterial;
    public Material sunsetMaterial;
    public Material nightMaterial;

    [Header("Global Ambient Lighting")]
    public Gradient ambientLightColor;
    
    [Header("Celestial Pivots & Shadows")]
    public Transform sunPivot;
    public Transform moonPivot;
    public Light directionalLight;
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.2f;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    void Start()
    {
        timeOfDay = 0.15f; 
    }

    void Update()
    {

        timeOfDay += Time.deltaTime / fullCycleDuration;
        if (timeOfDay >= 1f) timeOfDay -= 1f; 

        float dawnAlpha = 0f, dayAlpha = 0f, sunsetAlpha = 0f, nightAlpha = 0f;

        if (timeOfDay >= 0.8f) {
            dawnAlpha = SmoothBlend(timeOfDay, 0.8f, 1.0f);
            nightAlpha = 1f - dawnAlpha;
        } 
        else if (timeOfDay < 0.2f) {
            dayAlpha = SmoothBlend(timeOfDay, 0.0f, 0.2f);
            dawnAlpha = 1f - dayAlpha;
        } 
       
        else if (timeOfDay >= 0.2f && timeOfDay < 0.35f) {
            dayAlpha = 1f;
        } 
       
        else if (timeOfDay >= 0.35f && timeOfDay < 0.4f) {
            sunsetAlpha = SmoothBlend(timeOfDay, 0.35f, 0.4f);
            dayAlpha = 1f - sunsetAlpha;
        } 
        
        else if (timeOfDay >= 0.4f && timeOfDay < 0.45f) {
            nightAlpha = SmoothBlend(timeOfDay, 0.4f, 0.45f);
            sunsetAlpha = 1f - nightAlpha;
        } 
       
        else if (timeOfDay >= 0.45f && timeOfDay < 0.8f) {
            nightAlpha = 1f;
        }

        dawnAlpha = Mathf.Clamp01(dawnAlpha * 1.2f);
        dayAlpha = Mathf.Clamp01(dayAlpha * 1.2f);
        sunsetAlpha = Mathf.Clamp01(sunsetAlpha * 1.2f);
        nightAlpha = Mathf.Clamp01(nightAlpha * 1.2f);

        
        SetPlaneAlpha(dawnSky, dawnAlpha);
        SetPlaneAlpha(daySky, dayAlpha);
        SetPlaneAlpha(sunsetSky, sunsetAlpha);
        SetPlaneAlpha(nightSky, nightAlpha);
        
       
        currentCloudOffset += cloudScrollSpeed * Time.deltaTime;

        SetCloudAlphaAndScroll(dawnClouds, dawnAlpha, currentCloudOffset);
        SetCloudAlphaAndScroll(dayClouds, dayAlpha, currentCloudOffset);
        SetCloudAlphaAndScroll(sunsetClouds, sunsetAlpha, currentCloudOffset);
        SetCloudAlphaAndScroll(nightClouds, nightAlpha, currentCloudOffset);

        
        Material currentMat = dayMaterial;
        if (dawnAlpha > Mathf.Max(dayAlpha, sunsetAlpha, nightAlpha)) currentMat = dawnMaterial;
        else if (sunsetAlpha > Mathf.Max(dawnAlpha, dayAlpha, nightAlpha)) currentMat = sunsetMaterial;
        else if (nightAlpha > Mathf.Max(dawnAlpha, dayAlpha, sunsetAlpha)) currentMat = nightMaterial;

        if (currentMat != null)
        {
            foreach (MeshRenderer obj in objectsToChange)
            {
                if (obj != null && obj.sharedMaterial != currentMat) obj.sharedMaterial = currentMat;
            }
        }

        RenderSettings.ambientLight = ambientLightColor.Evaluate(timeOfDay);

        if (sunPivot != null && moonPivot != null && directionalLight != null)
        {
            if (timeOfDay < 0.1f) {
                sunPivot.localRotation = Quaternion.Euler(Mathf.Lerp(0f, 90f, timeOfDay / 0.1f), -90f, 90f);
                directionalLight.transform.rotation = sunPivot.rotation;
                directionalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, timeOfDay / 0.1f);
            } else if (timeOfDay >= 0.1f && timeOfDay < 0.35f) {
                sunPivot.localRotation = Quaternion.Euler(90f, -90f, 90f); 
                directionalLight.transform.rotation = sunPivot.rotation;
                directionalLight.intensity = dayIntensity;
            } else if (timeOfDay >= 0.35f && timeOfDay <= 0.5f) {
                float dropProgress = (timeOfDay - 0.35f) / 0.15f;
                sunPivot.localRotation = Quaternion.Euler(Mathf.Lerp(90f, 180f, dropProgress), -90f, 90f);
                directionalLight.transform.rotation = sunPivot.rotation;
                directionalLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dropProgress);
            } else {
                sunPivot.localRotation = EulerSafe(-90f, -90f, 90f); 
            }

            if (timeOfDay < 0.5f) {
                moonPivot.localRotation = EulerSafe(-90f, -90f, 90f); 
            } else if (timeOfDay >= 0.5f && timeOfDay < 0.6f) {
                moonPivot.localRotation = Quaternion.Euler(Mathf.Lerp(0f, 90f, (timeOfDay - 0.5f) / 0.1f), -90f, 90f);
                directionalLight.transform.rotation = moonPivot.rotation; 
            } else if (timeOfDay >= 0.6f && timeOfDay < 0.9f) {
                moonPivot.localRotation = Quaternion.Euler(90f, -90f, 90f); 
                directionalLight.transform.rotation = moonPivot.rotation;
            } else if (timeOfDay >= 0.9f && timeOfDay <= 1.0f) {
                moonPivot.localRotation = Quaternion.Euler(Mathf.Lerp(90f, 180f, (timeOfDay - 0.9f) / 0.1f), -90f, 90f);
                directionalLight.transform.rotation = moonPivot.rotation;
            }
        }
    }

    private float SmoothBlend(float time, float start, float end)
    {
        float t = Mathf.InverseLerp(start, end, time);
        return Mathf.SmoothStep(0f, 1f, t); 
    }

    private Quaternion EulerSafe(float x, float y, float z) { return Quaternion.Euler(x, y, z); }
    
    private void SetPlaneAlpha(MeshRenderer plane, float alpha)
    {
        if (plane != null && plane.material != null)
        {
            Color c = plane.material.GetColor(BaseColorID);
            c.a = alpha;
            plane.material.SetColor(BaseColorID, c);
            plane.enabled = (alpha > 0.01f);
        }
    }

    private void SetCloudAlphaAndScroll(MeshRenderer plane, float alpha, Vector2 offset)
    {
        if (plane != null && plane.material != null)
        {
            Color c = plane.material.GetColor(BaseColorID);
            c.a = alpha;
            plane.material.SetColor(BaseColorID, c);
            plane.material.SetTextureOffset("_BaseMap", offset); 
            plane.enabled = (alpha > 0.01f);
        }
    }
}