using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{

    public Sprite tick;

    public Image musicButton;
    public Image soundButton;

    private SettingsSave settings = new SettingsSave();

    public Text vers;

    private int credIter = 0;
    private bool inCredits = false;

    public string destination = "/settings.dat";

    public Text versionNum;

    public void Start()
    {
        
        soundButton.gameObject.SetActive(settings.Sound);
        musicButton.gameObject.SetActive(settings.Music);
        LoadData();
        vers.text = UnityEngine.Application.version;
    }

    public void Awake()
    {
        destination = Application.persistentDataPath + destination;
    }

    public void Back()
    {
            UIAnimationManager.Instance.ShowSettings(false);
            return;
    }

    public void ToggleMusic()
    {
        if (settings.Music)
        {
            settings.Music = false;
            SoundEffectHandler.Instance.PlaySound("clop");
        }
        else
        {
            settings.Music = true;
            SoundEffectHandler.Instance.PlaySound("clip");
        }
        SetMusic(settings.Music);
        Save();
    }

    public bool SetSounds(bool play)
    {
        if (SoundEffectHandler.Instance == null) return false;

        if (play)SoundEffectHandler.Instance.PlayOn();
        else SoundEffectHandler.Instance.PlayOff();
        soundButton.gameObject.SetActive(play);
        return true;
    }

    public bool SetMusic(bool play)
    {
        if (BackgroundMusic.Instance == null) return false;

        if (play) BackgroundMusic.Instance.UnMute();
        else BackgroundMusic.Instance.Mute();
        musicButton.gameObject.SetActive(play);
        return true;
    }

    public void ToggleSounds()
    {
        if (settings.Sound)
        {
            settings.Sound = false;
            SoundEffectHandler.Instance.PlaySound("clop");
        }
        else
        {
            settings.Sound = true;
            SoundEffectHandler.Instance.PlaySound("clip");
        }
        SetSounds(settings.Sound);
        Save();
    }

    public void OpenTutorial()
    {
        TutorialManager.Instance.StartMainTutorial();
    }

    public void OpenAbout()
    {
        Application.OpenURL("https://www.pocketpalsapp.com/about-1/");
    }

    public void Save()
    {
        FileStream file;

        //try and get the save game file
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = ResetFile();


        //serialise and save our data
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, settings);
        file.Close();
    }

    IEnumerator ApplyStartUpSettings()
    {
        bool sound = false;
        bool music = false;
        while (!sound || !music)
        {
            if (!sound)
            {
                sound = SetSounds(settings.Sound);
            }
            if (!music)
            {
                music = SetMusic(settings.Music);
            }
            yield return new WaitForSeconds(0.1f);
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
            settings = (SettingsSave)bf.Deserialize(file);
            file.Close();

            if (settings == null)
            {
                settings = new SettingsSave();
                
            }
            //apply settings
            StartCoroutine(ApplyStartUpSettings());
        }
        catch(System.Exception ex)
        {
            file.Close();
            Debug.Log(ex +"Failed to read exsisting load file. Creating a new one");
            ResetFile();
        }
    }

    public FileStream ResetFile()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        return File.Create(destination);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
[System.Serializable]
public class SettingsSave
{
    public int m = 1;
    public int s = 1;
    public bool Music  {
        get
        {
            return m == 1;
        }
        set
        {
            if (value) m = 1;
            else m = 0;
        }
    }
    public bool Sound
    {
        get
        {
            return s == 1;
        }
        set
        {
            if (value) s = 1;
            else s = 0;
        }
    }
}
