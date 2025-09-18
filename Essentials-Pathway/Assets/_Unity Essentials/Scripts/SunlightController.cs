using UnityEngine;

public class SunlightController : MonoBehaviour
{
    [Header("Day/Night Cycle Settings")]
    [Tooltip("How many real seconds for a full day to pass")]
    public float dayDurationInSeconds = 120f; // 2 minutes for a full day by default

    [Header("Optional: Light Intensity Control")]
    [Tooltip("Enable to automatically adjust light intensity based on sun angle")]
    public bool controlLightIntensity = true;
    [Tooltip("Maximum light intensity during noon")]
    public float maxLightIntensity = 1.0f;
    [Tooltip("Minimum light intensity during night")]
    public float minLightIntensity = 0.1f;

    [Header("Optional: Light Color Control")]
    [Tooltip("Enable to change light color throughout the day")]
    public bool controlLightColor = true;
    [Tooltip("Light color during sunrise/sunset")]
    public Color sunriseColor = new Color(1f, 0.6f, 0.2f); // Orange
    [Tooltip("Light color during noon")]
    public Color noonColor = Color.white;
    [Tooltip("Light color during night")]
    public Color nightColor = new Color(0.2f, 0.3f, 0.8f); // Blue

    private Light sunLight;
    private float rotationSpeed;

    void Start()
    {
        sunLight = GetComponent<Light>();

        // Calculate rotation speed: 360 degrees divided by day duration
        rotationSpeed = 360f / dayDurationInSeconds;
    }

    void Update()
    {
        // Rotate the light around the X-axis to simulate sun movement
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);

        // Optional: Control light intensity and color based on sun angle
        if (sunLight != null)
        {
            UpdateLightProperties();
        }
    }

    private void UpdateLightProperties()
    {
        // Get the sun's angle (how high it is in the sky)
        // When X rotation is 0-180, sun is above horizon
        // When X rotation is 180-360, sun is below horizon
        float sunAngle = transform.eulerAngles.x;

        // Normalize to 0-1 where 1 is noon, 0 is midnight
        float dayProgress;
        if (sunAngle <= 180f)
        {
            // Sun is above horizon (day time)
            dayProgress = 1f - (sunAngle / 180f);
        }
        else
        {
            // Sun is below horizon (night time)
            dayProgress = 0f;
        }

        // Control light intensity
        if (controlLightIntensity)
        {
            sunLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, dayProgress);
        }

        // Control light color
        if (controlLightColor)
        {
            Color currentColor;

            if (dayProgress > 0.8f) // High noon
            {
                currentColor = noonColor;
            }
            else if (dayProgress > 0.2f) // Day time
            {
                // Blend between sunrise and noon colors
                float t = (dayProgress - 0.2f) / 0.6f;
                currentColor = Color.Lerp(sunriseColor, noonColor, t);
            }
            else if (dayProgress > 0f) // Sunrise/sunset
            {
                // Blend between night and sunrise colors
                float t = dayProgress / 0.2f;
                currentColor = Color.Lerp(nightColor, sunriseColor, t);
            }
            else // Night time
            {
                currentColor = nightColor;
            }

            sunLight.color = currentColor;
        }
    }

    // Helper method to manually set time of day (0 = midnight, 0.5 = noon)
    [ContextMenu("Set to Noon")]
    public void SetToNoon()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    [ContextMenu("Set to Midnight")]
    public void SetToMidnight()
    {
        transform.rotation = Quaternion.Euler(180, 0, 0);
    }

    [ContextMenu("Set to Sunrise")]
    public void SetToSunrise()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}