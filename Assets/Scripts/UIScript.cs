using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour {
    public Rigidbody viewer;

    private TMP_Text text;

    void Start () {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = $"Position ({viewer.position.x.ToString("0.00").Replace(",",".")},{viewer.position.y.ToString("0.00").Replace(",", ".")},{viewer.position.z.ToString("0.00").Replace(",", ".")})\nSpeed: {viewer.linearVelocity.magnitude.ToString("0.00").Replace(",", ".")}";
    }
}
