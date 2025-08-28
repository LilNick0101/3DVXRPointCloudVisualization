using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SaveConfirmationPopup : MonoBehaviour {
    [SerializeField] private GameObject popupPanel;   // riferimento al pannello intero
    [SerializeField] private TMP_Text messageText;  // il TextMeshPro per il messaggio
    [SerializeField] private Button okButton;     // il bottone OK

    void Awake() {
        // Assicurati che sia nascosto
        popupPanel.SetActive(false);

        // Listener per chiudere
        okButton.onClick.AddListener(Hide);
    }

    public void Show(string message, float autoCloseAfter = 0f) {
        messageText.text = message;
        popupPanel.SetActive(true);

        if (autoCloseAfter > 0f)
            StartCoroutine(AutoHide(autoCloseAfter));
    }

    public void Hide() {
        popupPanel.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator AutoHide(float seconds) {
        yield return new WaitForSeconds(seconds);
        Hide();
    }
}