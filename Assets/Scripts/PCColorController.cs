using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestisce tre slider R, G, B per generare un colore 0–255 e applicarlo a tutti i punti.
/// </summary>
public class PCColorController : MonoBehaviour {
    [Header("UI Controls")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public TMP_Text redValueText;
    public TMP_Text greenValueText;
    public TMP_Text blueValueText;

    [Header("Point Cloud")]
    public PointCloudVisualizer pointCloudVisualizer;

    [Header("Material Property")]
    public string colorPropName = "_Color";

    private Color currentColor;

    void Awake() {
        // Imposto range e listener per ogni slider
        SetupChannel(redSlider, redValueText);
        SetupChannel(greenSlider, greenValueText);
        SetupChannel(blueSlider, blueValueText);

        redSlider.value = 255;
        greenSlider.value = 255;
        blueSlider.value = 255;
    }

    private void SetupChannel(Slider slider, TMP_Text label) {
        slider.minValue = 0;
        slider.maxValue = 255;
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(v => UpdateColor());
    }

    private void UpdateColor() {
        // Calcolo il colore normalizzato 0–1
        currentColor = new Color(
            redSlider.value / 255f,
            greenSlider.value / 255f,
            blueSlider.value / 255f
        );

        // Applico a tutti i punti
        ApplyColorToCloud();
    }

    private void ApplyColorToCloud() {
        if (pointCloudVisualizer == null || pointCloudVisualizer.points == null)
            return;

        pointCloudVisualizer.SetNewColor(currentColor);
    }
}
