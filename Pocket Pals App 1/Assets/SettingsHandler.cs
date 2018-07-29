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
            SoundEffectHandler.Instance.PlaySound("clop");
            BackgroundMusic.Instance.StopMusic();
            musicButton.gameObject.SetActive(false);
        }
        else
        {
            settings.music = true;
            SoundEffectHandler.Instance.PlaySound("clip");
            BackgroundMusic.Instance.StartBackgroundMusic();
            musicButton.gameObject.SetActive(true);
        }
    }

    public void ToggleSounds()
    {
        if (settings.sound)
        {
            settings.sound = false;
            SoundEffectHandler.Instance.PlaySound("clop");
            SoundEffectHandler.Instance.PlayOff();
            soundButton.gameObject.SetActive(false);
        }
        else
        {
            settings.sound = true;
            SoundEffectHandler.Instance.PlayOn();
            SoundEffectHandler.Instance.PlaySound("clip");
            soundButton.gameObject.SetActive(true);
        }
    }

    public void OpenTutorial()
    {
        TutorialManager.Instance.StartMainTutorial();
    }

    public void OpenAbout()
    {
        Application.OpenURL("https://www.pocketpalsapp.com/about-1/");
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
