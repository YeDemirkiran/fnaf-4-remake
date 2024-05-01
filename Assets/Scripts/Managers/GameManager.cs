using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Place
{
    public string Name;
    public Color Color;
    public Transform Transform;

    public Door door;
    public Animatronic animatronic;

    public Vector2 xClamp, yClamp;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Place[] places;
    public static Place[] Places { get; private set; }

    bool cursorVisible = false;

    // References
    UIManager uiManager;
    Tweener currentTweener;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        SetCursor(false);

        Places = places;

        foreach (var place in Places)
        {
            if (place.animatronic != null)
            {
                place.animatronic.animatronicPlace = place;
            }
        }
    }

    private void Start()
    {
        uiManager = UIManager.Instance;

        GameFadeIn(2f);
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

    public void GameFadeIn(float duration, bool enablePlayerController = true)
    {
        if (currentTweener != null)
        {
            currentTweener.Kill();
            currentTweener = null;
        }

        PlayerController.Instance.SetControlState(enablePlayerController);

        Color colorTransparent = new Color(0f, 0f, 0f, 0f);
        Color colorVisible = new Color(0f, 0f, 0f, 1f);

        currentTweener = DOVirtual.Color(colorVisible, colorTransparent, duration, (value) => { uiManager.overlay.color = value; });
    }

    public void GameFadeOut(float duration, bool disablePlayerController = true)
    {
        if (currentTweener != null)
        {
            currentTweener.Kill();
            currentTweener = null;
        }

        PlayerController.Instance.SetControlState(!disablePlayerController);

        Color colorTransparent = new Color(0f, 0f, 0f, 0f);
        Color colorVisible = new Color(0f, 0f, 0f, 1f);

        currentTweener = DOVirtual.Color(colorTransparent, colorVisible, duration, (value) => { uiManager.overlay.color = value; });
    }
}