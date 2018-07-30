using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ResourceSpotManager : MonoBehaviour
{
    static public ResourceSpotManager Instance { set; get; }

    private List<ResourceSpotSave> usedSpots;

    private List<ResourceSpotParent> activeResourceSpots = new List<ResourceSpotParent>();
    private List<ResourceSpotParent> hiddenResourceSpots = new List<ResourceSpotParent>();

    private bool shouldSpawn = true;

    //Ref to the map, to set the parent of the resourceSpots once they spawn
    private GPS gpsMap;

    //Used for girl distance and To get the gps map
    public GameObject girl;

    public GameObject ResourceSpotPrefab;

    public float spread = 1.3f;

    public int number = 15;

	public float MaxNumOfUsedSpots = 15;

    public float CoolDownTimeSeconds = 300.0f;

    public float checkThreshold = 0.01f;

    public string destination = "/data.dat";

    bool startedSpawning = false;

    // Use this for initialization
    void Start()
    {
        usedSpots = new List<ResourceSpotSave>();
        gpsMap = girl.GetComponent<GPS>();
        LoadData();
        for (int i = 0; i < number + 1; i++)
        {
            ResourceSpotParent rsp = Instantiate(ResourceSpotPrefab).GetComponent<ResourceSpotParent>();
            rsp.Hide();
            hiddenResourceSpots.Add(rsp);
        }
    }

    void FixedUpdate()
    {
        if (startedSpawning == false) return;

		float exess = usedSpots.Count - MaxNumOfUsedSpots;
		if(exess >= 1)
		{
			for(int j =0; j < exess; j++)
			{
				ResourceSpotSave rss = usedSpots [j];
				usedSpots.Remove (rss);
				TryUpdateResourceSpot (rss);
			}
			SaveData ();
		}
        for(int i = 0; i<  usedSpots.Count; i++)
        {
            ResourceSpotSave rss = usedSpots[i];
            if (rss.cooldown <= 0.0f)
            {
                usedSpots.Remove(rss);
                SaveData();
                TryUpdateResourceSpot(rss);
            }
            else { rss.cooldown -= Time.deltaTime; }
        }
    }
    
    public void AddNewSaveData(ResourceSpotParent rsp)
    {
        ResourceSpotSave rss = new ResourceSpotSave(rsp.spawnLoc, CoolDownTimeSeconds);
        usedSpots.Add(rss);
        SaveData();

    }

    private void Awake()
    {
        Instance = this;
        destination = Application.persistentDataPath + destination;
    }

    public IEnumerator Spawn()
    {

        while (shouldSpawn)
        {
            SaveData();
            //check to make sure we have refs to the required classes
            if (GPS.Insatance == null || GPS.Insatance.currentMap == null || ContentGenerator.Instance == null) yield return new WaitForSeconds(4);

            List<Vector2> LatLonPositions = ContentGenerator.Instance.GenerateResourceSpots(GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y, number);

            if (LatLonPositions != null)
            {
                startedSpawning = true;
                DespawnAll();
                foreach (Vector2 v2 in LatLonPositions)
                {
                    if (hiddenResourceSpots.Count < 1) break;

                    Vector3 spawnpos = GPS.Insatance.GetWorldPos(v2.x, v2.y);
                    spawnpos.y = ResourceSpotPrefab.transform.position.y;

                    ResourceSpotParent rsp = hiddenResourceSpots[0];
                    hiddenResourceSpots.Remove(rsp);

                    rsp.Show(spawnpos, v2);

                    if (IsResourceSpotUsed(v2))
                    {
                        rsp.OldUsed();
                    }

                    rsp.transform.parent = GPS.Insatance.currentMap.transform;
                    activeResourceSpots.Add(rsp);
                }

                ContentGenerator.Instance.WipeResouceSpots();
            }

            yield return new WaitForSeconds(4);

        }
    }

    private void DespawnAll()
    {
        for (int i = 0; i < activeResourceSpots.Count; i++)
        {
            ResourceSpotParent rsp = activeResourceSpots[i];
            activeResourceSpots.Remove(rsp);
            rsp.Hide();
            hiddenResourceSpots.Add(rsp);
        }
    }

    private void TryUpdateResourceSpot(ResourceSpotSave rss)
    {
        foreach (ResourceSpotParent rsp in activeResourceSpots)
        {
            if (rss.IsMatch(rsp.spawnLoc, checkThreshold))
            {
                rsp.ActiveAgain();
            }
        }
    }

    private bool IsResourceSpotUsed(Vector2 latlon)
    {
        foreach (ResourceSpotSave rss in usedSpots)
        {
            if (rss.IsMatch(latlon, checkThreshold))
            {
                return true;
            }
        }
        return false;
    }

    public float GetTimeOfCoolDown(ResourceSpotParent rsp)
    {
        foreach (ResourceSpotSave rss in usedSpots)
        {
            if (rss.IsMatch(rsp.spawnLoc, checkThreshold)) return rss.cooldown;
        }
        rsp.ActiveAgain();
        return 0.0f;
    }

    void SaveData()
    {
        FileStream file;

        //try and get the save game file
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = ResetFile();
        

        //serialise and save our data
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, new Savedata(usedSpots));
        file.Close();
    }

    public FileStream ResetFile()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        return File.Create(destination);

    }

    void LoadData()
    {
        FileStream file;

        //try and get save game file 
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            //if the file does not exsist create new local data class and save it;
            Debug.Log("File not found");

            ResetFile();

            //exit load
            return;
        }
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            Savedata sd = (Savedata)bf.Deserialize(file);
            usedSpots = sd.usedSpots;
            file.Close();

            if (usedSpots == null)
            {
                usedSpots = new List<ResourceSpotSave>();
                Debug.Log("resource spot list null ref");
            }
        }
        catch
        {
            file.Close();
            Debug.Log("Failed to read exsisting load file. Creating a new one");
            ResetFile();
        }
    }
}
[System.Serializable]
public class Savedata
{
    public bool ShouldScanPPals = true;
    public List<ResourceSpotSave> usedSpots;
    public Savedata(List<ResourceSpotSave>rs) { usedSpots = rs; }
}
