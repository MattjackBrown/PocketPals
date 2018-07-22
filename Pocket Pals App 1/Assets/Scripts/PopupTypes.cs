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
    private string str;
    public float countAlpha;

    public ExpDrop(int ex)
    {
        exp = ex;
        ID = 1;
    }
    public void SetText()
    {
        str = ((int)(exp * countAlpha)).ToString() + "exp";
        text.text = str;
    }

}
