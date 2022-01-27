using DG.Tweening;
using UnityEngine;

public class GameCard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private SpriteRenderer back;
    
    private Sprite faceSprite;
    private Sprite backSprite;
    public FaceSign faceSign;
    public FaceColor faceColor;

    public GameController controller;

    public int id;
    public void SetData(CardGameModel card, int id, GameController controller)
    {
        Texture2D ftx = card.faceImage;
        Texture2D btx = card.backImage;
        faceSprite = Sprite.Create(ftx, new Rect(0f, 0f, ftx.width, ftx.height), new Vector2(0.5f, 0.5f), GameConstants.PixelsPerUnit);
        backSprite = Sprite.Create(btx, new Rect(0f, 0f, btx.width, btx.height), new Vector2(0.5f, 0.5f), GameConstants.PixelsPerUnit);

        faceSign = card.faceSign;
        faceColor = card.faceColor;
        this.controller = controller;
        face.sprite = faceSprite;
        back.sprite = backSprite;

        this.id = id;
    }

    public void Tap()
    {
        controller.FlipFaceCard(this);
        FlipFace();
    }

    public void FlipFace()
    {
        AnimateFlipFace();
    }

    public void FlipBack()
    {
        AnimateFlipBack();
    }

    private void AnimateFlipFace()
    {
        transform.DORotate(Vector3.up * 180f, GameConstants.CardFlipAnimationTime);
        transform.DOScale(new Vector3(
            GameConstants.OpenCardScale, 
            GameConstants.OpenCardScale, 
            GameConstants.OpenCardScale), 
            GameConstants.CardFlipAnimationTime);
    }
    private void AnimateFlipBack()
    {
        transform.DORotate(Vector3.up * 0f, GameConstants.CardFlipAnimationTime);
        transform.DOScale(new Vector3(
            GameConstants.CloseCardScale,
            GameConstants.CloseCardScale, 
            GameConstants.CloseCardScale), 
            GameConstants.CardFlipAnimationTime);
    }
}
