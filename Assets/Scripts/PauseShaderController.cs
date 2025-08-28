using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;                // <- TextMeshPro namespace

public class PointCloudShaderUI : MonoBehaviour {
    [Header("Materials")]
    public Material[] shaderMaterial;     // Custom/PointCloudEffects
    private int currentIdx = -1;

    [Header("UI Components")]
    public TMP_Dropdown shaderDropdown; // il TMP_Dropdown “ShaderDropdown”

    [Header("Point Cloud")]
    public PointCloudVisualizer points;

    private Material selectedShader;

    void Start() {

        if (shaderDropdown == null && points == null)
            return;

        selectedShader = new Material(shaderMaterial[0]);

        // Popolo il TMP_Dropdown
        shaderDropdown.ClearOptions();
        var options = new List<string> {
             "Standard", "Stone", "Metal", "Glass",
        };

        // TextMeshPro richiede OptionData
        var data = new List<TMP_Dropdown.OptionData>();
        foreach (var s in options)
            data.Add(new TMP_Dropdown.OptionData(s));
        shaderDropdown.AddOptions(data);

        // Setup listener
        shaderDropdown.onValueChanged.AddListener(OnEffectSelected);

        // Stato iniziale
        OnEffectSelected(shaderDropdown.value);
    }

    private void OnEffectSelected(int idx) {
        if (selectedShader == null)
            return;

        if (currentIdx == idx)
            return;

        selectedShader = shaderMaterial[idx];

        points.pointMaterial = selectedShader;
        foreach (var p in points.points)
            p.gameObject.GetComponent<Renderer>().material = selectedShader;
        points.ResetColors();

     
    }
}
