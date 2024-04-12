using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool cursorVisible = false;

    // Start is called before the first frame update
    void Awake()
    {
        SetCursor(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursor(!cursorVisible);
        }
    }

    // Update is called once per frame
    void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
        cursorVisible = visible;
    }
}