using TMPro;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public byte currentNight { get; private set; } = 0; // 0 is prototype

    public float currentTime { get; private set; } = 0f;
    [Tooltip("In hours")] public float nightLengthInGame = 6f; // In hours
    [Tooltip("In minutes")] public float nightLengthRealLife = 9f; // In minutes

    TMP_Text debugHourText;

    private void Start()
    {
        debugHourText = UIManager.Instance.hourText;
    }

    void Update()
    {
        currentTime += Time.deltaTime * (nightLengthInGame * 3600f / (nightLengthRealLife * 60f));

        byte minute = (byte)((currentTime / 60f) % 60f);
        byte hour = (byte)((currentTime / 3600f) % nightLengthInGame);

        string minuteString;
        string hourString;
        if (minute < 10) { minuteString = "0" + minute.ToString(); }
        else { minuteString = minute.ToString(); }

        if (hour < 10) { hourString = "0" + hour.ToString(); }
        else { hourString = hour.ToString(); }

        debugHourText.text = hourString + ":" + minuteString;
    }
}