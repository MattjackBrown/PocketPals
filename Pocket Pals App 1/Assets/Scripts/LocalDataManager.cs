﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocalDataManager : MonoBehaviour {

    //used to return an instance of a local data manager
    public static LocalDataManager Instance{set; get;}

    private string dataFileName = "/data.dat";
    private string destination;
    private GameData localData;

	// Use this for initialization
	void Start ()
    {
        //This makes the local datamanager accessible from anywhere
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        destination = Application.persistentDataPath + dataFileName;
        LoadData();
    }

    public void UpdateName(string newName)
    {
        localData.Username = newName;
        SaveData();
    }

   public void UpdateDistance(float delta)
    {
        localData.DistanceTravelled += delta;
        SaveData();
    }


    public void AddPocketPal(GameObject obj)
    {
       //Add the pocketPal to the players inventory
        localData.Inventory.AddPocketPal(obj.GetComponent<PocketPalParent>());

        //increas the players EXP
        localData.IncreaseExp(obj.GetComponent<PocketPalParent>().GetAnimalData().GetExp());

        SaveData();
    }

    public void ResetLocalData()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        localData = new GameData();
    }

    //try and get a players pocketpal. If player does not own Pocket pal with ID returns null
    public PocketPalData TryGetPocketPal(int ID)
    {
        foreach (PocketPalData ppd in localData.Inventory.GetMyPocketPals())
        {
            if (ppd.ID == ID) return ppd;
        }
        return null;
    }

    public PocketPalInventory GetInventory()
    {
        return localData.Inventory;
    }

    void SaveData()
    {
        try
        {
            FileStream file;

            //try and get the save game file
            if (File.Exists(destination)) file = File.OpenWrite(destination);
            else file = ResetFile();

            //serialise and save our data
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, localData);
            file.Close();
        }
        catch(System.Exception ex)
        {
            Debug.Log("Failed to save" + ex);
        }
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
            localData = (GameData)bf.Deserialize(file);
            file.Close();

            Debug.Log(localData.DistanceTravelled);
            Debug.Log(localData.Username);
            localData.Inventory.PrintMyPocketPals();
        }
        catch
        {
            file.Close();
            Debug.Log("Failed to read exsisting load file. Creating a new one");
            ResetFile();
        }
    }

    public FileStream ResetFile()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        localData = new GameData();
        return File.Create(destination);

    }

    public GameData GetData() { return localData; }
}
