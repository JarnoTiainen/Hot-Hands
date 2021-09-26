using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Sirenix.OdinInspector;
using SimpleJSON;
public class WebSocketService : MonoBehaviour
{
    WebSocket websocket;

    public const string ThrowOp = "5";
    public const string SummonMonsterOp = "PlayCard";


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
            Debug.Log("OnMessage!");

            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);
            
            //GameMessage newMessage = JsonUtility.FromJson<GameMessage>(message);
            //Debug.Log(newMessage);

            // getting the message as a string
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
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
        PlayCard playCard = new PlayCard("OnPlayCard", PlayCard.CardSource.Hand, 3);

        SendWebSocketMessage(JsonUtility.ToJson(playCard));
    }
}
