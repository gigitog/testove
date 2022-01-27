using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

public class ServerController : MonoBehaviour
{
    private const string DataUrl = "https://drive.google.com/uc?export=download&id=1tkYtCNDo3v1HKfMSly_Mzi3NfCOGzi-T";
    private string dataRaw = "";
    private JsonDataModel data;

    public event EventHandler<RespondData> OnGetData;
    public event EventHandler<RespondCards> OnGetCards;

    public void GetCardsData()
    {
        StartCoroutine(GetData());
    }

    public void GetRandomCards()
    {
        if (dataRaw == "") return;
        List<CardTextModel> randomCardsText = new List<CardTextModel>();
        // --- Random DATA
        for (int i = 0; i < GameConstants.CardsNum / 2; i++)
        {
            var randomId = UnityEngine.Random.Range(0, data.list.Count);
            randomCardsText.Add(data.list[randomId]);
        }
        // ---
        
        StartCoroutine(GetCardsImages(randomCardsText));
    }
    
    # region Server Requsets
    private IEnumerator GetData()
    {
        var request = UnityWebRequest.Get(DataUrl);
        
        yield return request.SendWebRequest();
        
        var arg = new RespondData()
        {
            data = request.downloadHandler.text,
            isError = false
        };
        
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError("Error! " + request.error);
            arg.isError = true;
        }
        else
        {
            dataRaw = arg.data = request.downloadHandler.text;
            data = JsonConvert.DeserializeObject<JsonDataModel>(dataRaw);
            Debug.Log(JsonConvert.SerializeObject(data));
        }
        request.Dispose();

        OnGetData?.Invoke(this, arg);
        
    }

    private IEnumerator GetCardsImages(List<CardTextModel> randomCardsText)
    {
        var arg = new RespondCards()
        {
            data = new List<CardGameModel>(),
            table = null,
            isError = false
        };
        Texture2D backImage = null;
        
        // --- get back of card
        var getBack = UnityWebRequestTexture.GetTexture(data.backUrl);
        yield return getBack.SendWebRequest();
        
        if(getBack.isNetworkError || getBack.isHttpError) {
            Debug.LogError(getBack.error);
            arg.isError = true;
        }
        else
        {
            backImage = ((DownloadHandlerTexture) getBack.downloadHandler).texture;
        }
        getBack.Dispose();
        // --- ---
        
        // --- get table 
        var getTable = UnityWebRequestTexture.GetTexture(data.tableUrl);
        yield return getTable.SendWebRequest();
        if(getTable.isNetworkError || getTable.isHttpError) {
            Debug.LogError(getTable.error);
            arg.isError = true;
        }
        else
        {
            arg.table =  ((DownloadHandlerTexture) getTable.downloadHandler).texture;
        }
        getTable.Dispose();
        // --- ---
        
        // --- get cards' images
        foreach (var card in randomCardsText)
        {
            var request = UnityWebRequestTexture.GetTexture(card.faceImageUrl);
            yield return request.SendWebRequest();

            if(request.isNetworkError || request.isHttpError) {
                Debug.LogError(request.error);
                arg.isError = true;
            }
            else
            {
                var cardobj = new CardGameModel()
                {
                    faceColor = GameHelper.ConvertColor(card),
                    faceSign = GameHelper.ConvertSign(card),
                    faceImage = ((DownloadHandlerTexture) request.downloadHandler).texture,
                    backImage = backImage
                };
                arg.data.Add(cardobj);
            }
            request.Dispose();
        }
        Debug.Log("Random cards obtained!");
        // --- ---
        
        OnGetCards?.Invoke(this, arg);
    }
    # endregion
}
