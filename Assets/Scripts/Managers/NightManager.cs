using TMPro;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public byte currentNight { get; private set; } = 0; // 0 is prototype

    public float currentTime { get; private set; } = 0f;
    public float nightSpeed = 60f; // Value 1 is identical to real life. Make it 60 to make 1 Hour = 1 Minute IRL

    TMP_Text debugHourText;

    private void Start()
    {
        debugHourText = UIManager.Instance.hourText;
    }

    void Update()
    {
        currentTime += Time.deltaTime * nightSpeed;

        byte minute = (byte)((currentTime / 60f) % 60f);
        byte hour = (byte)((currentTime / 3600f) % 12f);

        string minuteString;
        string hourString;
        if (minute < 10) { minuteString = "0" + minute.ToString(); }
        else { minuteString = minute.ToString(); }

        if (hour < 10) { hourString = "0" + hour.ToString(); }
        else { hourString = hour.ToString(); }

        debugHourText.text = hourString + ":" + minuteString;
    }
}