using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//this class is used to update all the text in the tracks and trails menu
public class TrackAndTrailsHandle : MonoBehaviour
{
    public TrackAndTrailsHandle Instance { set; get; }

    public Text distanceTravelledText;

    public GameObject GuessLayer;

    public int basePixelate = 100;

    public GameObject CollectionLayer;
    public GameObject[] Collection;

    public GameObject InspectLayer;
    public Image InspectBar;
    public Image InspectImage;

    public TrackData activeTrack;
    public List<TrackData> allTracks;

    private void Start()
    {
        Instance = this;
    }

    public void SetInspectlayer()
    {
        GuessLayer.SetActive(false);
        CollectionLayer.SetActive(false);
        InspectLayer.SetActive(true);

        InspectImage.sprite = AssetManager.Instance.GetTrackByID(activeTrack.ID).identifier;

        InspectBar.fillAmount = activeTrack.GetFloatDone();
        
        InspectImage.sprite = PixelateTexture(InspectImage,(int)((1- activeTrack.GetFloatDone()) * basePixelate));
    }

    public void ChangeActiveTrack(int index)
    {
        if (allTracks.Count <= index)
        {
            activeTrack = allTracks[index];
            SetInspectlayer();
        }
    }

    public void SetToDefault()
    {
        GuessLayer.SetActive(false);
        CollectionLayer.SetActive(true);
        InspectLayer.SetActive(false);
        RefreshCollection();
    }

    public void RefreshCollection()
    {
        allTracks = LocalDataManager.Instance.GetData().TrackInv.GetTracks();
        for (int i = 0; i < allTracks.Count; i++)
        {
            Collection[i].SetActive(true);
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
                        avg = (avg + tex2D.GetPixel(i, j))/2;
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

    public void Guess()
    {

    }
}