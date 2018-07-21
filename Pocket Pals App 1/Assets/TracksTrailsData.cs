using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackData
{
    public float distTarget = 1000.0f;
    public float startDistance = 0.0f;
    public int ID = 0;
    public string uID = "";

    public TrackData() { }

    public TrackData(int i, float sd,float targ)
    {
        ID = i;
        startDistance = sd;
        distTarget = targ;
        uID = System.Guid.NewGuid().ToString();
    }

    public float GetFloatDone(float currentDistance)
    {
        Debug.Log("Start: " + startDistance);
        Debug.Log("Current: " + currentDistance);
        float delta = (currentDistance - startDistance)*1000;
        return delta / distTarget;
    }
}
[System.Serializable]
public class TracksAndTrailsPreset
{
    public int ID = 0;
    public float distMin = 1000.0f;
    public float distMax = 1000.0f;
    public string PocketPalName = "None";
    public int PocketPalID = 0;
    public Sprite identifier;

    public float expMin = 4000.0f;
    public float expMax = 6000.0f;

    public float rarity = 1.0f;

    public float GetCompletedExp()
    {
        return Random.Range(expMin, expMax);
    }

    public float GetRequiredDistance()
    {
        return Random.Range(distMin, distMax);
    }
}
[System.Serializable]
public class TracksInventory
{
    private List<TrackData> tracks = new List<TrackData>();

    public List<TrackData> GetTracks()
    {
        return tracks;
    }
    public bool TryAddTrack(TrackData td)
    {
        if (tracks.Count < 6)
        {
            tracks.Add(td);
            return true;
        }
        else
        {
            return false;
        }
    }

}