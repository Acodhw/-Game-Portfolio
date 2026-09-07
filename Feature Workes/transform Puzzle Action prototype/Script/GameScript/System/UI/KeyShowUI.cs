using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyShowUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("보여줄 키 코드")]
    private string keyCode;

    [SerializeField]
    [Tooltip("몇 번째 키 소스인지 확인")]
    private int index;

    private TMP_Text showTx;

    private RectTransform img;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        showTx = GetComponent<TMP_Text>();
        img = transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        string s = InputSystem.actions.FindAction(keyCode).GetBindingDisplayString(index, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        img.sizeDelta = new Vector2(20, 20);
        if (s.Equals("Left")) s = "←";
        else if (s.Equals("Right")) s = "→";
        else if (s.Equals("Up")) s = "↑";
        else if (s.Equals("Down")) s = "↓";
        else if (s.Equals("Escape")) { s = "esc"; img.sizeDelta = new Vector2(20, 20); }
        else if (s.Equals("Space")) { s = " "; img.sizeDelta = new Vector2(50, 20); }
        else if (s.Equals("Left Shift")) { s = "LShift"; img.sizeDelta = new Vector2(40, 20); }
        else if (s.Equals("Right Shift")){ s = "RShift"; img.sizeDelta = new Vector2(40, 20); }
        else if (s.Equals("Shift")) { img.sizeDelta = new Vector2(40, 20); }
        else if (s.Equals("Control")) { s = "Ctrl"; img.sizeDelta = new Vector2(40, 20); }
        else if (s.Equals("Tab")) { img.sizeDelta = new Vector2(30, 20); }
        showTx.text = s;
        }


}
