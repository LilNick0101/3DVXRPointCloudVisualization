using UnityEngine;

public class PointCloudManipulation : MonoBehaviour
{

    [Header("Point cloud manipulation")]
    public KeyCode increaseSize = KeyCode.KeypadPlus;
    public KeyCode decreaseSize = KeyCode.KeypadMinus;

    float sizeUp = 0f;
    float sizeDown = 0f;

    PointCloudVisualizer pointCloud;

    void Start() {
        pointCloud = GetComponent<PointCloudVisualizer>();
    }

    void FixedUpdate() {
        SizeManipulation();
    }

    void GetInput() {
        sizeUp = Input.GetKey(increaseSize) ? 1f : 0f;
        sizeDown = Input.GetKey(decreaseSize) ? 1f : 0f;
    }

    void SizeManipulation() {
        pointCloud.IncrementPointSize((sizeUp / 100f) - (sizeDown / 100f));
    }

    private void Update() {
        GetInput();
    }
}
