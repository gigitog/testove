using System;
using System.Collections.Generic;
using UnityEngine;

public class RespondData : EventArgs
{
    public bool isError;
    public string data;
}

public class RespondCards : EventArgs
{
    public bool isError;
    public Texture2D table;
    public List<CardGameModel> data;
}
