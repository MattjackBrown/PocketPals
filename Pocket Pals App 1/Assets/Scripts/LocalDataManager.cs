using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocalDataManager : MonoBehaviour {

    //used to return an instance of a local data manager
    public static LocalDataManager Instance{set; get;}

    private string statsFileName = "/stats.dat";
    private GameData localData;

	// Use this for initialization
	void Start ()
    {
        //This makes the local datamanager accessible from anywhere
        Instance = this;
        DontDestroyOnLoad(gameObject);

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

    void SaveData()
    {
        string destination = Application.persistentDataPath + statsFileName;
        FileStream file;

        //try and get the save game file
        if (File.Exists(destination)) file = File.OpenWrite(destination);

        //create if non exsists
        else file = File.Create(destination);

        //serialise and save our data
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, localData);
        file.Close();
    }

    void LoadData()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        //try and get save game file 
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            //if the file does not exsist create new local data class and save it;
            Debug.Log("File not found");
            localData = new GameData();
            SaveData();

            //exit load
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        localData = (GameData)bf.Deserialize(file);
        file.Close();

        Debug.Log(localData.DistanceTravelled);
        Debug.Log(localData.Username);
    }

    public GameData GetData() { return localData; }
}
