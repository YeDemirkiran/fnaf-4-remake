using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Image overlay;

    public GameObject playerActionPanel;
    public TMP_Text playerActionText;

    public GameObject hourPanel;
    public TMP_Text hourText;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
}