using TMPro;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    public byte currentNight { get; private set; } = 0; // 0 is prototype
    bool nightComplete = false;

    public float currentTime { get; private set; } = 0f;
    [Tooltip("In hours")] public float nightLengthInGame = 6f; // In hours
    [Tooltip("In minutes")] public float nightLengthRealLife = 9f; // In minutes

    public bool debugOn = false;

    GameObject debugPanel;
    TMP_Text debugHourText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        debugPanel = UIManager.Instance.hourPanel;
        debugHourText = UIManager.Instance.hourText;

        debugPanel.SetActive(debugOn);
    }

    void Update()
    {
        if (!nightComplete)
        {
            currentTime += Time.deltaTime * (nightLengthInGame * 3600f / (nightLengthRealLife * 60f));

            byte minute = (byte)(currentTime / 60f % 60f);
            byte hour = (byte)(currentTime / 3600f);
            //byte hour = (byte)((currentTime / 3600f) % nightLengthInGame);

            if (debugOn)
            {
                string minuteString;
                string hourString;
                if (minute < 10) { minuteString = "0" + minute.ToString(); }
                else { minuteString = minute.ToString(); }

                if (hour < 10) { hourString = "0" + hour.ToString(); }
                else { hourString = hour.ToString(); }

                debugHourText.text = hourString + ":" + minuteString;
            }

            if (hour >= nightLengthInGame)
            {
                GameManager.Instance.GameFadeOut(3f);
                nightComplete = true;
            }
        }      
    }
}