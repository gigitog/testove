public static class GameHelper
{
    public static FaceColor ConvertColor(CardTextModel card)
    {
        switch (card.faceColor)
        {
            case "red": return FaceColor.Red;
            case "orange": return FaceColor.Orange;
            case "green": return FaceColor.Green;
            case "blue": return FaceColor.Blue;
            case "violet": return FaceColor.Violet;
            case "dark": return FaceColor.Dark;
            default: return FaceColor.Dark; 
        }
    }

    public static FaceSign ConvertSign(CardTextModel card)
    {
        switch (card.faceSign)
        {
            case "square": return FaceSign.Square;
            case "circle": return FaceSign.Circle;
            case "triangle": return FaceSign.Triangle;
            default:  return FaceSign.Square; 
        }
    }
}
