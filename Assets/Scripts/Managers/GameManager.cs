using System;
using UnityEngine;

[Serializable]
public class Place
{
    public string Name;
    public Transform Transform;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] Place[] places;
    public static Place[] Places { get; private set; }

    bool cursorVisible = false;

    // Start is called before the first frame update
    void Awake()
    {
        SetCursor(false);

        Places = places;
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