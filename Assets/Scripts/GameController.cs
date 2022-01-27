using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // UI
    [SerializeField] private List<Transform> cardsTransforms;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameCard openCard1 = null;
    [SerializeField] private GameCard openCard2 = null;
    [SerializeField] private Transform cardsParent;
    [SerializeField] private Text counter;
    [SerializeField] private CanvasGroup counterBox;
    [SerializeField] private Text loadingText;
    [SerializeField] private Image table;
    [SerializeField] private Image darkPanel;
    

    // Controllers
    public bool pause;
    public bool isGameStarted;
    public int counterNum;
    private ServerController sc;

    public float gapH = 5f;
    public float gapV = 1.5f;

    private Camera cam;

    private void Start()
    {
        // Setting default data
        cam = Camera.main;
        counter.text = "0";
        isGameStarted = false;
        counterBox.gameObject.SetActive(false);
        counterBox.alpha = 0f;
        pause = true;
        sc = GetComponent<ServerController>();
        sc.OnGetCards += Draw;
        sc.OnGetData += ScOnGetData;
        sc.GetCardsData();
    }
    
    #region Game Logic
    private void Update()
    {
        if (!isGameStarted && Input.GetMouseButtonDown(0))
            StartGame();

        if (pause) return; // don't process click if game on pause
        
        if (Input.GetMouseButtonDown(0))
            HandleTap();
    }

    private void HandleTap()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name.StartsWith("CardObject"))
            {
                Debug.Log("It's a card!");
                Pause();
                hit.collider.GetComponent<GameCard>().Tap();
            }
        }
    }

    private void ScOnGetData(object sender, RespondData e)
    {
        // Subscribed method to event of getting json data from server
        if (e.isError)
        {
            ShowError();
        }
        else
        {
            loadingText.text = "Tap to play!";
        }
    }

    private void StartGame()
    {
        counter.text = "0";
        counterNum = 0;
        sc.GetRandomCards();
        Debug.Log("Game starts! Wait for images!");
        isGameStarted = true;
        FadeLoadingText(0f);
    }

    private void EndGame()
    {
        loadingText.text = "Good job! Tap if you want to restart!";
        FadeLoadingText(1f);
        isGameStarted = false;
        cardsTransforms.Clear();
    }
    
    public void FlipFaceCard(GameCard card)
    {
        StartCoroutine(WaitForFlip(card));
    }

    private IEnumerator WaitForFlip(GameCard card)
    {
        Pause();
        yield return new WaitForSeconds(GameConstants.CardFlipAnimationTime);
        if (openCard1 == null)
        {
            openCard1 = card;
        }
        else
        {
            openCard2 = card;
            if (CheckMatch())
                SameCards();
            else
                DifferentCards();
        }

        if (counterNum == GameConstants.CardsNum / 2)
        {
            EndGame();
        }
        else
        {
            Unpause();
        }
    }
    
    private bool CheckMatch()
    {
        if (openCard1 == null || openCard2 == null)
            Debug.Log("Мы тут не должны были оказаться!!!");

        return (openCard1.faceColor == openCard2.faceColor && openCard1.faceSign == openCard2.faceSign);
    }

    private void SameCards()
    {
        Debug.Log("Одинаковые карты! Ура!");
        DestroyCards();
        counterNum++;
        counter.text = $"{counterNum}";
    }

    private void DifferentCards()
    {
        Debug.Log("Разные карты! Блин((");
        openCard1.FlipBack();
        openCard2.FlipBack();
        openCard1 = openCard2 = null;
    }
    
    private void DestroyCards()
    {
        Destroy(openCard1.gameObject);
        Destroy(openCard2.gameObject);
        openCard1 = openCard2 = null;
    }
    
    private void Pause()
    {
        pause = true;
    }

    private void Unpause()
    {
        pause = false;
    }
    
    #endregion
    
    # region Interface

    private void FadeLoadingText(float value)
    {
        loadingText.DOFade(value, GameConstants.TimeToStart);
    }

    private void ShowError()
    {
        loadingText.text = "Connection error!";
        FadeLoadingText(1f);
    }

    private void Draw(object sender, RespondCards e)
    {
        Debug.Log("Images obtained! Drawing UI!");
        if (e.isError)
        {
            ShowError();
            Debug.Log("Show UI Error");
            return;
        }

        // --- Set table image
        table.sprite = Sprite.Create(e.table, new Rect(0f, 0f, e.table.width, e.table.height), Vector2.zero, GameConstants.PixelsPerUnit);
        // ---

        // --- random fill
        List<CardGameModel> cardOrder = new List<CardGameModel>();
        
        foreach (var card in e.data)
        {
            cardOrder.Add(card);
            cardOrder.Add(card);
        }
        for (int i = 0; i < cardOrder.Count; i++) {
            var temp = cardOrder[i];
            int randomIndex = UnityEngine.Random.Range(i, cardOrder.Count);
            cardOrder[i] = cardOrder[randomIndex];
            cardOrder[randomIndex] = temp;
        }
        // ---
        
        // --- Instantiation of card Objects
        for (int i = 0; i < GameConstants.CardsNum; i++)
        {
       
            var cardObject = Instantiate(cardPrefab, cardsParent);
            cardObject.GetComponent<GameCard>().SetData(cardOrder[i], i, this);
            cardsTransforms.Add(cardObject.transform);
            cardObject.SetActive(false);
        }
        // ---
        
        // --- Drawing a counter circle
        counterBox.gameObject.SetActive(true);
        counterBox.DOFade(1f, GameConstants.TimeToStart);
        // ---
        
        darkPanel.DOFade(0f, 1f);
        
        // Positioning cards
        StartCoroutine(CardPrettify());
    }

    private IEnumerator CardPrettify()
    {
        // Method of positioning cards
        
        // Meaning of variables check in the bottom of the document

        var cardsNumInRow = GameConstants.CardsNum / 2;
        bool isOdd = (cardsNumInRow % 2 == 1); // is odd num of cards in row (always true)
        
        for (int i = 0; i < GameConstants.CardsNum; i++)
        {
            float posX;
            float posY;
            var horizontalShift = -gapH * (cardsNumInRow - 1) / 2;

            if (i < cardsNumInRow)
            {
                posX = horizontalShift + i * gapH;
                posY = gapV;
            }
            else
            {
                posX = horizontalShift + (i-cardsNumInRow) * gapH;
                posY = -gapV;
            }
            
            cardsTransforms[i].localPosition = new Vector3(posX, posY, 0f);
            yield return new WaitForSeconds(GameConstants.TimeToCardAppear);
            cardsTransforms[i].gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(GameConstants.TimeToStart);
        StartCoroutine(ShowCardsToRemember());
    }

    private IEnumerator ShowCardsToRemember()
    {
        // method showing cards for a little for memorizing
        foreach (var card in cardsTransforms)
        {
            card.GetComponent<GameCard>().FlipFace();
        }

        yield return new WaitForSeconds(GameConstants.CardFlipAnimationTime);

        yield return new WaitForSeconds(GameConstants.TimeToRemember);

        foreach (var card in cardsTransforms)
        {
            card.GetComponent<GameCard>().FlipBack();
        }
        yield return new WaitForSeconds(GameConstants.CardFlipAnimationTime);
        
        Unpause();
    }

    #endregion
}

/* gapH - horizontal distance between cards' centers
 * gapV - vertical distance between cards' centers
 * horizontal shift - X position of the first left card
 * isOdd - is odd number of cards in a row (for task it 3, so always true)
 *
 *                      Center (x = 0)
 *                          |
 *     |<---horizonShift--->|
 *     |                    |
 *     |                    |
 * #########            #########            #########
 * ####0####            ####0####            ####0####---------
 * #########            #########            #########       ↑
 *     |                    |                    |           | 
 *     |<------ gapH ------>|<------ gapH ------>|          gapV
 *     |                    |                    |           |
 * #########            #########            #########       ↓
 * ####0####            ####0####            ####0####---------
 * #########            #########            #########
 */

