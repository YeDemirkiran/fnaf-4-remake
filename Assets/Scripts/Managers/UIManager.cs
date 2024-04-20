using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject playerActionPanel;
    public TMP_Text playerActionText;

    public TMP_Text hourText;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
}