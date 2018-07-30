using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { set; get; }
    public GameObject TutorialObj;
    public Image currentPage;
    private List<Sprite> currentList;
   

    int iter = 0;

    public List<Sprite> mainTutorial;


    public List<Sprite> Tracks;

    public List<Sprite> MiniGame;

    public void Start()
    {
        Instance = this;
        TutorialObj.SetActive(false);
    }

    public void StartMainTutorial()
    {
        iter = 0;
        currentPage.sprite = mainTutorial[0];
        currentList = mainTutorial;
        TutorialObj.SetActive(true);

    }

    public void StartTracksTutorial()
    {
        iter = 0;
        currentPage.sprite = Tracks[0];
        currentList = Tracks;
        TutorialObj.SetActive(true);
    }

    public void StartMiniGame()
    {
        iter = 0;
        currentPage.sprite = MiniGame[0];
        currentList = MiniGame;
        TutorialObj.SetActive(true);
    }

    public void Next()
    {
        if (currentList == null) TutorialObj.SetActive(false);
        iter++;
        if (iter > currentList.Count-1)
        {
            TutorialObj.SetActive(false);
            return;
        }
        currentPage.sprite = currentList[iter];
    }
    public void Previous()
    {
        if (currentList == null) TutorialObj.SetActive(false);
        iter--;
        if (iter < 0)
        {
            iter = 0;
        }
        currentPage.sprite = currentList[iter];
    }
}

