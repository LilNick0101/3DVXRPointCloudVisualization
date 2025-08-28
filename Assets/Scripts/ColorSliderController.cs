using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSliderController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("UI References")]
    [Tooltip("The Slider that goes from 0 to 255")]
    public Slider slider;

    [Tooltip("The Text or TMP_Text that displays the slider's value")]
    public TMP_Text tmpText;

    void Awake() {

        // Aggancia il listener
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Imposta subito il testo iniziale
        OnSliderValueChanged(slider.value);
    }

    void OnSliderValueChanged(float rawValue) {
        int intValue = Mathf.RoundToInt(rawValue);

        if (tmpText != null)
            tmpText.text = intValue.ToString();

    }
}
