using UnityEngine;

public class PauseController : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas pauseCanvas;
    public MonoBehaviour viewerCamera;

    void Start() {
        pauseCanvas.enabled = false;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void TogglePause() {
        mainCanvas.enabled = !mainCanvas.enabled;
        pauseCanvas.enabled = !mainCanvas.enabled;
        Cursor.visible = !mainCanvas.enabled;
        Cursor.lockState = pauseCanvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        viewerCamera.enabled = mainCanvas.enabled;
    }
}
