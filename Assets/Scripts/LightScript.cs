using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightScript : MonoBehaviour {
    [Header("Sun Settings")]
    [SerializeField] private Light sunLight;
    [SerializeField] private float cycleSpeed = 10f; // gradi/sec

    [Header("Day Colors")]
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);
    public Color sunriseColor = new Color(1f, 0.5f, 0.3f);
    public Color dayColor = Color.white;

    [Header("UI")]
    [SerializeField] private Toggle autoCycleToggle;
    [SerializeField] private Slider hourSlider;   // 0..23
    [SerializeField] private TMP_Text hourLabel;

    private float sunAngle;
    private bool manualMode;

    void Start() {
        // Inizializza l'angolo in base alla rotazione già presente
        sunAngle = (sunLight.transform.rotation.eulerAngles.x + 360f) % 360f;

        // Slider 0..23
        hourSlider.minValue = 0;
        hourSlider.maxValue = 23;
        hourSlider.wholeNumbers = true;
        hourSlider.onValueChanged.AddListener(OnHourChanged);

        // Toggle auto/manual
        autoCycleToggle.onValueChanged.AddListener(isOn => {
            manualMode = !isOn;
            hourSlider.interactable = !isOn;
        });

        // Parto in auto-cycle
        autoCycleToggle.isOn = false;
        manualMode = true;

        // Sync UI
        UpdateUIFromAngle();
    }

    void Update() {
        if (manualMode == false) {
            // Avanza l'angolo in avanti, mai indietro
            sunAngle = (sunAngle + cycleSpeed * Time.deltaTime) % 360f;

            // Applica la rotazione al sole
            sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);

            // Sync hour & colore
            UpdateUIFromAngle();
        }
    }

    // Chiamato dallo slider
    private void OnHourChanged(float value) {
        manualMode = true;            // entri in manual mode
        autoCycleToggle.isOn = false; // disattiva il toggle

        int hour = Mathf.RoundToInt(value);

        sunAngle = (hour / 24f) * 360f - 90f;
        if (sunAngle < 0f) sunAngle += 360f;

        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);

        UpdateSunAppearance(sunAngle);

        hourLabel.text = "Hour: " + hour.ToString("00");
    }

    private void UpdateUIFromAngle() {
        float aligned = (sunAngle + 90f) % 360f;
        int hour = Mathf.FloorToInt(aligned / 360f * 24f);

        hourSlider.SetValueWithoutNotify(hour);
        hourLabel.text = "Hour: " + hour.ToString("00");

        UpdateSunAppearance(sunAngle);
    }

    private void UpdateSunAppearance(float angle) {
        float t = Mathf.InverseLerp(0f, 360f, angle);

        if (angle < 90f)
            sunLight.color = Color.Lerp(sunriseColor, dayColor, t);
        else if (angle < 180f)
            sunLight.color = Color.Lerp(dayColor, sunriseColor, t);
        else if (angle < 270f)
            sunLight.color = Color.Lerp(sunriseColor, nightColor, t);
        else
            sunLight.color = Color.Lerp(nightColor, sunriseColor, t);

        sunLight.intensity = Mathf.Lerp(0.1f, 1.5f, Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void ResumeAutoCycle() {
        manualMode = false;
        autoCycleToggle.isOn = true;
    }
}