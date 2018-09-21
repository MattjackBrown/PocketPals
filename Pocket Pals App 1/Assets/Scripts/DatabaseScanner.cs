using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseScanner
{

    public static List<DefaultPocketPalInfo> ScanFileForInfo(TextAsset text)
    {
        List<DefaultPocketPalInfo> ppals = new List<DefaultPocketPalInfo>();

        using (var reader = new StreamReader(new MemoryStream(text.bytes)))
        {


            string HeaderRow = reader.ReadLine();

            string[] Headers = HeaderRow.Split(',');

            while (!reader.EndOfStream)
            {
                try
                {
                    DefaultPocketPalInfo info = new DefaultPocketPalInfo();
                    string line = reader.ReadLine();

                    string[] values = line.Split(',');

                    info.ID = Int32.Parse(values[0]);

                    switch (values[1].ToLower())
                    {
                        case "mammal":
                            info.ppalType = PPalType.Animal;
                            break;
                        case "bird":
                            info.ppalType = PPalType.Bird;
                            break;
                        case "insect":
                            info.ppalType = PPalType.Insect;
                            break;
                        case "amphibian":
                            info.ppalType = PPalType.Amphibian;
                            break;
                        case "reptile":
                            info.ppalType = PPalType.Reptile;
                            break;
                    }

                    info.PPalName = values[2];

                    //----- Habitat type ----\\

                    switch (values[3].ToLower())
                    {
                        case "woodland":
                            info.spawnType = SpawnType.Woodland;
                            break;
                        case "wetland":
                            info.spawnType = SpawnType.Wetland;
                            break;
                        case "coast":
                            info.spawnType = SpawnType.Coastal;
                            break;
                        case "Meadows":
                            info.spawnType = SpawnType.Meadows;
                            break;

                    }


                    info.LatinName = values[4];

                    info.order = values[5];

                    info.Rarity = Int32.Parse(values[6]);

                    //----- spawn Time ----\\

                    switch (values[7].ToLower())
                    {
                        case "night":
                            info.timeActive = SpawnTime.night;
                            break;
                        case "day":
                            info.timeActive = SpawnTime.day;
                            break;
                        case "all":
                            info.timeActive = SpawnTime.all;
                            break;

                    }

                    //values[8];


                    //----- PPal Weight ------\\

                    if (values[9] == "0")
                    {
                        info.minWeight = 0;
                        info.maxWeight = 0;
                    }
                    else
                    {
                        string[] vals = values[9].Split('-');
                        info.minWeight = float.Parse(vals[0]); 
                        info.maxWeight = float.Parse(vals[1]); 
                    }


                    ///----------- PPalLength -----\\\\\
                    string lengthString;
                    if (info.ppalType == PPalType.Bird) lengthString = values[11];
                    else lengthString = values[10];

                    if (lengthString == "0")
                    {
                        info.minLength = 0;
                        info.maxLength = 0;
                    }
                    else
                    {
                        string[] vals = lengthString.Split('-');
                        info.minLength = float.Parse(vals[0]);
                        info.maxLength = float.Parse(vals[1]);
                    }

                    info.PocketFact = values[12];
                    info.Description = values[13];

                    ppals.Add(info);
                    info.PrintMe();
                }
                catch(Exception ex)
                {
                    Debug.Log("Pocket pal failed to read Error: " + ex);
                }

            }
        }
        return ppals;
    }

}
