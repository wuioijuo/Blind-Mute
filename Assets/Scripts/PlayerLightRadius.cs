using UnityEngine;

public class PlayerLightRadius : MonoBehaviour
{
    [Header("Основные настройки")]
    public Light playerLight;

    [Header("Радиус света")]
    public float minRange = 5f;
    public float maxRange = 12f;

    [Header("Скорость изменения радиуса")]
    public float pulseSpeed = 1.5f;

    [Header("Яркость")]
    public float minIntensity = 1.2f;
    public float maxIntensity = 2.5f;

    [Header("Случайное мерцание")]
    public bool randomFlicker = true;
    public float flickerAmount = 0.6f;
    public float flickerSpeed = 12f;

    private float baseTimeOffset;

    private void Start()
    {
        if (playerLight == null)
        {
            playerLight = GetComponent<Light>();
        }

        baseTimeOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (playerLight == null)
        {
            return;
        }

        float t = Mathf.Sin((Time.time + baseTimeOffset) * pulseSpeed);
        t = (t + 1f) / 2f;

        float targetRange = Mathf.Lerp(minRange, maxRange, t);
        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        if (randomFlicker)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, baseTimeOffset);
            float flicker = Mathf.Lerp(-flickerAmount, flickerAmount, noise);

            targetRange += flicker;
            targetIntensity += flicker * 0.25f;
        }

        playerLight.range = Mathf.Clamp(targetRange, minRange, maxRange);
        playerLight.intensity = Mathf.Clamp(targetIntensity, minIntensity, maxIntensity);
    }
}
