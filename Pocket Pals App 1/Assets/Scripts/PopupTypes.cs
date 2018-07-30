using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupData
{
    public int ID = 0;
    public float Alpha = 0.0f;
}

public class ItemDrop : PopupData
{
    public Image img;
    public Sprite spr;

    public ItemDrop(Sprite s)
    {
        spr = s;
        ID = 0;
    }
}


public class ExpDrop : PopupData
{
    public Text text;
    public int exp;
    public bool isLevelUp;
    private string str;
    public float countAlpha;
    public bool PlayStars = true;

    public ExpDrop() { }

    public ExpDrop(int ex, bool level)
    {
        exp = ex;
        ID = 1;
        isLevelUp = level;
    }
    public void SetText()
    {
        str = ((int)(exp * countAlpha)).ToString() + "exp";
        text.text = str;
    }

}

