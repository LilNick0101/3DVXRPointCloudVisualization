using SimpleFileBrowser;
using UnityEngine;

public class PauseButtons : MonoBehaviour
{

    [Header("Point Cloud Manager")]
    public PointCloudVisualizer pointManager;
    
    public void DoExitGame() {
        Application.Quit();
    }

    public void SelectNewFIle() {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("PLY Files", ".ply"));
        FileBrowser.SetDefaultFilter(".ply");
        FileBrowser.ShowLoadDialog(
            (paths) => { Debug.Log(paths[0]); pointManager.plyFileName = paths[0]; pointManager.resetPoints(); },
            () => { Debug.Log("Load canceled"); },
            FileBrowser.PickMode.Files
        );
    }

}
