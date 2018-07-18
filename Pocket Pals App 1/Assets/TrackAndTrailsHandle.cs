using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//this class is used to update all the text in the tracks and trails menu
public class TrackAndTrailsHandle : MonoBehaviour
{
    public TrackAndTrailsHandle Instance { set; get; }

    public Text distanceTravelledText;

    private void Start()
    {
        Instance = this;
    }

}