using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//this class is used to update all the text in the tracks and trails menu
public class TrackAndTrailsHandle : MonoBehaviour
{

    public Text distanceTravelledText;


    public void UpdateText()
    {     
        distanceTravelledText.text = "Travelled: " + Math.Round(LocalDataManager.Instance.GetData().DistanceTravelled, 1) + "/kM";
    }
}
