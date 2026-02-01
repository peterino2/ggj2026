using UnityEngine;
using UnityEngine.InputSystem;

public class HelpMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
    }

    public GameObject HelpPanel;
    private bool visible = false;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            visible = !visible;
            HelpPanel.SetActive(visible);
            if (visible)
            {
                Time.timeScale = 0f;
            }
            if (!visible)
            {
                Time.timeScale = 1.0f;
            }
        }
    }
}
