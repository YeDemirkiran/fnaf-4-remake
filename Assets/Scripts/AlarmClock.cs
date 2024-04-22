using System;
using UnityEngine;

public class AlarmClock : MonoBehaviour
{
    [SerializeField] Digit[] digits;
    [SerializeField] Material ledOnMaterial, ledOffMaterial;

    // Use these for specific numbers
    public enum DisplayNumbers { off = 0, zero = 63, one = 6, two = 91, three = 79, four = 102, five = 109, six = 125, seven = 7, eight = 127, nine = 103 }

    // To convert in-game hour to enum
    public DisplayNumbers[] Numbers { get; private set; } = new DisplayNumbers[] { DisplayNumbers.zero, DisplayNumbers.one, DisplayNumbers.two,
    DisplayNumbers.three, DisplayNumbers.four, DisplayNumbers.five,
    DisplayNumbers.six, DisplayNumbers.seven, DisplayNumbers.eight, DisplayNumbers.nine, DisplayNumbers.off};

    NightManager nightManager;

    // The code above is wonky. Dictionaries would work better probably but I don't care enough to fix it.

    //const byte zero = 63 // a, b, c, d, e, f (0111111)
    //const byte one = 6; // b, c (0000110)
    //const byte two = 91; // a, b, g, d, e (1011011)
    //const byte three = 79; // a, b, g, c, d (1001111)
    //const byte four = 102; // b, c, g, f (1100110)
    //const byte five = 109; // a, f, g, c, d (1101101)
    //const byte six = 125; // a, f, g, c, d, e (1111101)
    //const byte seven = 7; // a, b, c (0000111)
    //const byte eight = 127; // a, b, c, d, e, f, g (1111111)
    //const byte nine = 103; // a, b, c, f, g (1100111)

    private void Start()
    {
        nightManager = NightManager.Instance;
    }

    // Start is called before the first frame update
    void Update()
    {
        SetTime(nightManager.currentHour, nightManager.currentMinute);
    }

    public void SetTime(int hour, int minute)
    {
        digits[0].SetDigit((byte)Numbers[hour / 10], ledOnMaterial, ledOffMaterial);
        digits[1].SetDigit((byte)Numbers[hour % 10], ledOnMaterial, ledOffMaterial);
        digits[2].SetDigit((byte)Numbers[minute / 10], ledOnMaterial, ledOffMaterial);
        digits[3].SetDigit((byte)Numbers[minute % 10], ledOnMaterial, ledOffMaterial);
    }
}

[Serializable]
public class Digit
{
    public GameObject a, b, c, d, e, f, g;

    // Not performant probably but whatever
    public void SetDigit(byte value, Material ledOnMaterial, Material ledOffMaterial)
    {
        GameObject[] leds = new GameObject[7] { a, b, c, d, e, f, g };

        bool[] bits = value.GetBit();

        for (int i = 0; i < 7; i++)
        {
            leds[i].GetComponent<Renderer>().material = bits[i] ? ledOnMaterial : ledOffMaterial;
        }
    }    
}

// Move this somewhere else later
public static class ByteExtensions
{
    public static bool GetBit(this byte b, int bitNumber)
    {
        bool bit = (b & (1 << bitNumber - 1)) != 0;
        return bit;
    }

    public static bool[] GetBit(this byte b)
    {
        bool[] bits = new bool[8] { b.GetBit(1), b.GetBit(2), b.GetBit(3), b.GetBit(4), b.GetBit(5), b.GetBit(6), b.GetBit(7), b.GetBit(8) };
        return bits;
    }
}