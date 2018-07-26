﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//this class is used to update all the text in the tracks and trails menu
public class TrackAndTrailsHandle : MonoBehaviour
{
    public static TrackAndTrailsHandle Instance { set; get; }

    public int basePixelate = 100;
    public float MaxBonusModifier = 5.0f;

    public GameObject CollectionLayer;
    public GameObject[] Collection;
    public Sprite EmptyGlass;
    public Sprite UsedGlass;


    public GameObject InspectLayer;
    public Image InspectBar;
    public Image InspectImage;
    public GameObject GuessButton;
    public GameObject CompleteButton;
    public Text DistanceLeft;

    public GameObject GuessLayer;
    public GuessClass[] GuessBoxes;
    public Sprite Tick;
    public Sprite Cross;
    private List<PocketPalParent> guessPalIDs = new List<PocketPalParent>();
    private PocketPalParent correctPPal;
    private GuessClass activeGuess;


    private TrackData activeTrack;
    public List<TrackData> allTracks;

    private void Start()
    {
        Instance = this;
    }

    public void SetInspectlayer()
    {
        InspectImage.sprite = AssetManager.Instance.GetTrackByID(activeTrack.ID).identifier;

        float cDist = LocalDataManager.Instance.GetData().DistanceTravelled;

        float complete = activeTrack.GetFloatDone(cDist);

        complete = Mathf.Clamp01(complete);

        InspectBar.fillAmount = complete;

        DistanceLeft.text = Math.Round(activeTrack.GetDistanceLeft(cDist), 1).ToString()+ " Km Left";

        InspectImage.sprite = PixelateTexture(InspectImage, (int)((1 - activeTrack.GetFloatDone(cDist)) * basePixelate));

        CheckForGuess();

        InspectLayer.SetActive(true);

    }

    public void CheckForGuess()
    {
        if (activeTrack.GuessID != -1)
        {
            GuessButton.SetActive(false);
            CompleteButton.SetActive(true);
        }
        else
        {
            GuessButton.SetActive(true);
            CompleteButton.SetActive(false);
        }
    }

    public void Back()
    {
        if (GuessLayer.activeSelf)
        {
            GuessLayer.SetActive(false);
            SetInspectlayer();
            return;
        }
        else if (InspectLayer.activeSelf)
        {
            InspectLayer.SetActive(false);
            RefreshCollection();
            return;
        }
        else if (CollectionLayer.activeSelf)
        {
            UIAnimationManager.Instance.OpenTracks(false);
            return;
        }
    }

    public void ChangeActiveTrack(int index)
    {

        if (allTracks.Count > index)
        {
            activeTrack = allTracks[index];
            SetInspectlayer();
        }
        else
        {
            NotificationManager.Instance.QuestionNotification("Would you like to use A Magnifying Glass?", TryUseMagnifyingGlass, null);
        }
    }

    public void SetToDefault()
    {
        GuessLayer.SetActive(false);
        CollectionLayer.SetActive(true);
        InspectLayer.SetActive(false);
        RefreshCollection();
    }

    public void GuessClicked()
    {
        GuessLayer.SetActive(true);
        int correctPPalID = AssetManager.Instance.GetTrackByID(activeTrack.ID).PocketPalID;
        correctPPal = AssetManager.Instance.GetPocketPalFromID(correctPPalID).GetComponent<PocketPalParent>();
        guessPalIDs = AssetManager.Instance.GetRandomPocketpals(GuessBoxes.Length - 1, correctPPalID);
        guessPalIDs.Add(correctPPal);
        guessPalIDs = guessPalIDs.OrderBy(x => UnityEngine.Random.Range(0, 100)).ToList();
        for (int i = 0; i < GuessBoxes.Length; i++)
        {
            GuessBoxes[i].Init(guessPalIDs[i], i);
        }
        ToggleGuess(0);
    }

    public void RefreshCollection()
    {
        allTracks = LocalDataManager.Instance.GetData().TrackInv.GetTracks();

        for (int j = 0; j < Collection.Length; j++)
        {
            Collection[j].SetActive(false);
            Collection[j].GetComponent<Image>().sprite = EmptyGlass;
        }

        int i = 0;
        for (i = 0; i < allTracks.Count; i++)
        {
            Collection[i].SetActive(true);
            Collection[i].GetComponent<Image>().sprite = UsedGlass;
        }

        if (allTracks.Count < Collection.Length)
        {
            Collection[i].SetActive(true);
            Collection[i].GetComponent<Image>().sprite = EmptyGlass;
        }
    }

    public void TryUseMagnifyingGlass()
    {
        if (LocalDataManager.Instance.GetItemInventory().UseItemWithID(GlobalVariables.MagnifyingGlassID))
        {
            LocalDataManager.Instance.TryAddTracks();
            RefreshCollection();
        }
        else
        {
            NotificationManager.Instance.InteractError("You do not have any Magnifying Glasses!");
        }
    }

    public Sprite PixelateTexture(Image img, int pixelateWidth)
    {
        Texture2D tex2D = new Texture2D(img.sprite.texture.width, img.sprite.texture.height, TextureFormat.ARGB32, false);
        tex2D.SetPixels32(img.sprite.texture.GetPixels32());
        tex2D.Apply();

        int xSquares = tex2D.width / pixelateWidth;
        int ySquares = tex2D.height / pixelateWidth;

        for (int xSquare = 0; xSquare < xSquares; xSquare++)
        {
            for (int ySquare = 0; ySquare < ySquares; ySquare++)
            {
                Color avg = new Color();
                int startX = xSquare * pixelateWidth;
                for (int i = startX; i < startX + pixelateWidth; i++)
                {
                    int startY = ySquare * pixelateWidth;
                    for (int j = startY; j < startY + pixelateWidth; j++)
                    {
                        avg = (avg + tex2D.GetPixel(i, j)) / 2;
                    }
                }

                for (int i = startX; i < startX + pixelateWidth; i++)
                {
                    int startY = ySquare * pixelateWidth;
                    for (int j = startY; j < startY + pixelateWidth; j++)
                    {
                        tex2D.SetPixel(i, j, avg);
                    }
                }
            }
        }

        tex2D.Apply();
        return Sprite.Create(tex2D, InspectImage.sprite.rect, Vector2.zero);
    }

    public void ToggleGuess(int index)
    {
        foreach (GuessClass gc in GuessBoxes)
        {
            gc.img.sprite = Cross;
        }
        GuessBoxes[index].img.sprite = Tick;
        activeGuess = GuessBoxes[index];
    }

    public void Guess()
    {
        NotificationManager.Instance.QuestionNotification("Are you sure you want to guess " + activeGuess.name.text + "?", LockInGuess, null);
    }

    public void LockInGuess()
    {


        float modifier = activeTrack.GetFloatDone(LocalDataManager.Instance.GetData().DistanceTravelled);
        modifier = 1 / modifier;
        if (modifier > MaxBonusModifier) modifier = MaxBonusModifier;

        activeTrack.GuessID = activeGuess.ppp.PocketPalID;
        activeTrack.Multiplier = modifier;

        NotificationManager.Instance.CustomHeaderNotification("Guess Confimed!", "You have guessed this track belongs too a " + activeGuess.name.text + " If you're right, you will receive an exp modifier of: " + Math.Round(activeTrack.Multiplier, 1) + "x EXP");

        ServerDataManager.Instance.WriteTrack(LocalDataManager.Instance.GetData(), activeTrack);

        Back();
        Back();
    }

    public void Complete()
    {
        if (activeTrack.GetFloatDone(LocalDataManager.Instance.GetData().DistanceTravelled) < 1)
        {
            NotificationManager.Instance.CustomHeaderNotification("Sorry You Cannot Do This Right Now!!", "You need to fully track this animal before you can claim your reward!");
            return;
        }
        if (activeTrack.GuessID == AssetManager.Instance.GetTrackByID(activeTrack.ID).PocketPalID)
        {
            NotificationManager.Instance.CongratsNotification("Correct! You have tracked the animal!");

            PocketPalParent ppp = AssetManager.Instance.GetPocketPalFromID(activeTrack.GuessID).GetComponent<PocketPalParent>();

            LocalDataManager.Instance.SuccessfulTrack(ppp, AssetManager.Instance.GetTrackByID(activeTrack.ID), activeTrack.Multiplier);
        }
        else
        {
            NotificationManager.Instance.CustomHeaderNotification("You Failed!","I am sorry, But you guessed wrong! You have lost the animals track!");
        }
        LocalDataManager.Instance.RemoveTracks(activeTrack);
        Back();
        Back();
    }
}

[System.Serializable]
public class GuessClass
{
    public Image img;
    public Text name;
    public PocketPalParent ppp;
    public int index = 0;

    public void Init(PocketPalParent parent, int i)
    {
        index = i;
        img.sprite = null;
        ppp = parent;
        name.text = parent.name;
    }
}