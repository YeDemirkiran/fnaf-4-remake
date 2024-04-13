using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLocation : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.currentPlace != null)
        {
            text.text = PlayerController.currentPlace.Name;
            text.color = PlayerController.currentPlace.Color;
        }
        else
        {
            text.text = "DEFAULT";
            text.color = Color.white;
        } 
    }
}