using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class StyleType
{
    public bool IsPaid = false;
    public string ID;
}
[System.Serializable]
public class TextureType:StyleType
{
    public Texture2D tex;
}
[System.Serializable]
public class MeshType :StyleType
{
    public MeshFilter mesh;
}
[System.Serializable]
public class MaterialType : StyleType
{
    public Material mat;
}
public class PoseType : StyleType
{
    public Animation anim;
}