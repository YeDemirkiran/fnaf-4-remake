using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject playerActionPanel;
    public TMP_Text playerActionText;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
}