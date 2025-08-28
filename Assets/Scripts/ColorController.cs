using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorController : MonoBehaviour {
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

        // Partenza a bianco (255,255,255)
        redSlider.value = 255;
        greenSlider.value = 255;
        blueSlider.value = 255;
        UpdateColor();
    }

    private void SetupChannel(Slider slider, TMP_Text label) {
        slider.minValue = 0;
        slider.maxValue = 255;
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(v => UpdateColor());
    }

    private void UpdateColor() {
        // Calcolo il colore normalizzato 0�1
        currentColor = new Color(
            redSlider.value / 255f,
            greenSlider.value / 255f,
            blueSlider.value / 255f
        );

        // Aggiorno le etichette
        redValueText.text = ((int)redSlider.value).ToString();
        greenValueText.text = ((int)greenSlider.value).ToString();
        blueValueText.text = ((int)blueSlider.value).ToString();

        // Applico a tutti i punti
        ApplyColorToCloud();
    }

    private void ApplyColorToCloud() {
        if (pointCloudVisualizer == null || pointCloudVisualizer.points == null)
            return;

        // Se usi un materiale condiviso per tutti i punti, basta:
        if (pointCloudVisualizer.pointMaterial != null) {
            pointCloudVisualizer.pointMaterial.SetColor(colorPropName, currentColor);
            return;
        }

        // Altrimenti lo applichiamo a ogni Renderer
        var block = new MaterialPropertyBlock();
        block.SetColor(colorPropName, currentColor);

        foreach (var pt in pointCloudVisualizer.points) {
            var rend = pt.gameObject.GetComponent<Renderer>();
            if (rend != null)
                rend.SetPropertyBlock(block);
        }
    }
}
