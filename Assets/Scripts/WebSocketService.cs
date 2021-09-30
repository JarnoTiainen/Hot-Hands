using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Sirenix.OdinInspector;
using SimpleJSON;
public class WebSocketService : MonoBehaviour
{
    public int playerNumber;

    public static WebSocketService Instance { get; private set; }

    static WebSocket websocket;

    public const string ThrowOp = "5";
    public const string SummonMonsterOp = "PlayCard";
    string playCard = "PLAYCARD";


    private void Awake()
    {
        Instance = gameObject.GetComponent<WebSocketService>();
    }

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket("wss://n14bom45md.execute-api.eu-north-1.amazonaws.com/production");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            JSONNode data = JSON.Parse(System.Text.Encoding.UTF8.GetString(bytes));
            Debug.Log(data);
            switch((string)data[0])
            {
                case "GETSIDE":
                    playerNumber = int.Parse(data[1]);
                    break;
                case "PLAYCARD":
                    Debug.Log("Message type was PLAYCARD");
                    PlayCardMessage playCard = new PlayCardMessage((int)data[1][0], data[1][1], data[1][2]);
                    Debug.Log(playCard.cardSource);
                    break;
                case "DRAWCARD":
                    Debug.Log("Message type was DRAWCARD");
                    DrawCardMessage drawCardMessage = new DrawCardMessage((int)data[1][0], data[1][1]);
                    if (drawCardMessage.player == playerNumber) Debug.Log("You draw " + drawCardMessage.card);
                    else Debug.Log("Opponent draws a card");
                    break;
                case "ATTACK":
                    Debug.Log("Message type was ATTACK");
                    break;
                case "SAVECARD":
                    Debug.Log("Saved card " + data[1][0] + " succesfully");
                    break;
                case "SETDECK":
                    if(int.Parse(data[1]) == 0)
                    {
                        Debug.Log("Deck save ok");
                    }
                    else
                    {
                        Debug.Log("Deck save failed, everything is ok");
                    }
                    break;
                default:
                    Debug.Log("Message type was UNKOWN");
                    break;
            }
        };
        Debug.Log("opening");
        await websocket.Connect();
    }

    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
    #endif

    }

    async static void SendWebSocketMessage(string message)
    {

        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log("sent");
            await websocket.SendText(message);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    [Button]
    public static void PlayCardMessage(int cardIndex)
    {
        PlayCardMessage playCardMessage = new PlayCardMessage(1, 3);
        string playCardMessageJSON = JsonUtility.ToJson(playCardMessage);

        GameMessage message = new GameMessage("OnMessage", "PLAYCARD", playCardMessageJSON);

        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    [Button] public static void GetPlayerNumber()
    {
        GameMessage playCard = new GameMessage("OnMessage", "GETSIDE", "");
        SendWebSocketMessage(JsonUtility.ToJson(playCard));
    }

    [Button]
    public static void DrawCard()
    {
        GameMessage message = new GameMessage("OnMessage", "DRAWCARD", "");
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    public static void SaveCardToDataBase(Card card)
    {
        GameMessage message = new GameMessage("OnMessage", "SAVECARD", card.CreateCardJSON());
        card.SaveCardToCardList();
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
    public static void SetDeck(string deckJson)
    {
        Debug.Log("Send deck data");

        GameMessage message = new GameMessage("OnMessage", "SETDECK", deckJson);
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
}
