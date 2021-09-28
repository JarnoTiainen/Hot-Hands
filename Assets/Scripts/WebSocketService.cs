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

    WebSocket websocket;

    public const string ThrowOp = "5";
    public const string SummonMonsterOp = "PlayCard";
    string playCard = "PLAYCARD";

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket("wss://ay9x6yqaea.execute-api.eu-north-1.amazonaws.com/dev");

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
            Debug.Log("Gained Server message!");
            JSONNode data = JSON.Parse(System.Text.Encoding.UTF8.GetString(bytes));
            Debug.Log(data);
            switch((string)data[0])
            {
                case "GETYOURPLAYERNUMBER":
                    playerNumber = int.Parse(data[1]);
                    break;
                case "PLAYCARD":
                    Debug.Log("Message type was PLAYCARD");
                    PlayCardMessage playCard = new PlayCardMessage((int)data[1][0], data[1][1], data[1][2]);
                    break;
                case "DRAWCARD":
                    Debug.Log("Message type was DRAWCARD");
                    break;
                case "ATTACK":
                    Debug.Log("Message type was ATTACK");
                    break;
                default:
                    Debug.Log("Message type was UNKOWN");
                    break;
            }
        };
        await websocket.Connect();
    }

    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
    #endif

        if (Input.GetKeyDown(KeyCode.A)) Throw();
    }

    async void SendWebSocketMessage(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    [Button]
    public void Throw()
    {
        GameMessage throwMessage = new GameMessage("OnMessage", ThrowOp);

        SendWebSocketMessage(JsonUtility.ToJson(throwMessage));
    }

    [Button]
    public void PlayCardMessage(int cardIndex)
    {
        PlayCardMessage playCard = new PlayCardMessage("OnMessage", 1, cardIndex);

        SendWebSocketMessage(JsonUtility.ToJson(playCard));
    }

    [Button] public void GetPlayerNumber()
    {
        GameMessage gameMessage = new GameMessage("OnGetSide", "message body");

        SendWebSocketMessage(JsonUtility.ToJson(gameMessage));
    }
}
