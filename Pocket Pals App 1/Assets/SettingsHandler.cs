using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{

    public Sprite tick;

    public Image musicButton;
    public Image soundButton;

    private SettingsSave settings = new SettingsSave();

    public Image CreditsLayer;
    public Sprite[] credits;
    private int credIter = 0;
    private bool inCredits = false;

    public Text versionNum;

    public void Start()
    {
        soundButton.gameObject.SetActive(settings.sound);
        musicButton.gameObject.SetActive(settings.music);
    }

    public void Back()
    {
        if (inCredits)
        {
            CreditsLayer.gameObject.SetActive(false);
            inCredits = false;
            return;
        }
        else
        {
            UIAnimationManager.Instance.ShowSettings(false);
            return;
        }
    }

    public void NextCred()
    {
        credIter++;
        if (credIter > credits.Length - 1)
        {
            credIter = 0;
        }
        CreditsLayer.sprite = credits[credIter];
    }

    public void ToggleMusic()
    {
        if (settings.music)
        {
            settings.music = false;
            musicButton.gameObject.SetActive(false);
        }
        else
        {
            settings.music = true;
            musicButton.gameObject.SetActive(true);
        }
    }

    public void ToggleSounds()
    {
        if (settings.sound)
        {
            settings.sound = false;
            soundButton.gameObject.SetActive(false);
        }
        else
        {
            settings.sound = true;
            soundButton.gameObject.SetActive(true);
        }
    }

    public void OpenTutorial()
    {
        TutorialManager.Instance.StartMainTutorial();
    }

    public void OpenAbout()
    {

    }

    public void OpenCredits()
    {
            CreditsLayer.gameObject.SetActive(true);
            inCredits = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class SettingsSave
{
    public bool music = true;
    public bool sound = true;
}
