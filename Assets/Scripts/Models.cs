using System.Collections.Generic;
using UnityEngine;

public class CardTextModel
{
    public string faceImageUrl;
    public string faceSign;
    public string faceColor;
}

public class JsonDataModel
{
    public string backUrl;
    public string tableUrl;
    public List<CardTextModel> list;
}

public class CardGameModel
{
    public Texture2D faceImage;
    public Texture2D backImage;
    public FaceSign faceSign;
    public FaceColor faceColor;
}

public enum FaceSign
{
    Square,
    Circle,
    Triangle
}

public enum FaceColor
{
    Red,
    Orange,
    Green,
    Blue,
    Violet,
    Dark
}

public class GameConstants
{
    public const int CardsNum = 6;
    public const float PixelsPerUnit = 10;
    public const float CardFlipAnimationTime = 0.6f;
    public const float TimeToStart = 2f;
    public const float TimeToRemember = 1.9f;
    public const float TimeToCardAppear = TimeToAllCardsAppear/CardsNum;
    public const float TimeToAllCardsAppear = 1.5f;
    public const float CloseCardScale = 0.9f;
    public const float OpenCardScale = 1.1f;
}
