using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugToggler : MonoBehaviour {

    public List<GameObject> DebugStuff;
    private bool IsDebug = false;

    public void ToggleDebug()
    {
        //flip flop for the toggle
        if (IsDebug) IsDebug = false;
        else IsDebug = true;

        //loop through and set activeness
        foreach (GameObject obj in DebugStuff)
        {
            obj.SetActive(IsDebug);
        }
    }
}
