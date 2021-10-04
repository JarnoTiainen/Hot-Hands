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

    [SerializeField]private Deck deck;
    [SerializeField] private MonsterZone yourMonsterZone;
    [SerializeField] private MonsterZone enemyMonsterZone;
    public static WebSocketService Instance { get; private set; }
    static WebSocket websocket;
    [SerializeField] private bool debuggerModeOn = false;

    private void Awake()
    {
        Instance = gameObject.GetComponent<WebSocketService>();
    }

    // Start is called before the first frame update
    void Start()
    {
        websocket = new WebSocket("wss://n14bom45md.execute-api.eu-north-1.amazonaws.com/production");
        OpenNewConnection();

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            deck.SendDeckData();
            GetPlayerNumber();
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
            Debug.Log("server message: " + data[0] + " " + data[1]);
            switch((string)data[0])
            {
                case "GETSIDE":
                    playerNumber = int.Parse(data[1]);
                    break;
                case "PLAYCARD":
                    if(debuggerModeOn) Debug.Log("Message type was PLAYCARD");
                    PlayCardMessage playCardMessage = JsonUtility.FromJson<PlayCardMessage>(data[1]);

                    if(playCardMessage.player == playerNumber)
                    {
                        Debug.LogWarning("You should add update card stats here :_3");
                        yourMonsterZone.UpdateCardData(true, playCardMessage);
                    }
                    else
                    {
                        Debug.LogWarning(playCardMessage.cardName + " Enemy card index: " + playCardMessage.boardIndex);
                        enemyMonsterZone.AddNewMonsterCard(false, playCardMessage.boardIndex);
                        enemyMonsterZone.UpdateCardData(false, playCardMessage);
                    }
                    break;
                case "DRAWCARD":
                    if (debuggerModeOn) Debug.Log("Message type was DRAWCARD");

                    DrawCardMessage drawCardMessage = JsonUtility.FromJson<DrawCardMessage>(data[1]);
                    if (drawCardMessage.player == playerNumber)
                    {
                        Hand.RevealNewCard(drawCardMessage);
                    }
                    else
                    {
                        EnemyHand.AddNewCard();
                    }
                    break;
                case "ATTACK":
                    if (debuggerModeOn) Debug.Log("Message type was ATTACK");
                    AttackEventMessage attackEventMessage = JsonUtility.FromJson<AttackEventMessage>(data[1]);
                    attackEventMessage.attackerValues = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.attacker);
                    attackEventMessage.targetValues = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.target);
                    if(debuggerModeOn) Debug.Log("attacked index: " + attackEventMessage.attackerValues.index +" target index: " + attackEventMessage.targetValues.index + "attacker lp: " + attackEventMessage.attackerValues.lp + " rp: " + attackEventMessage.attackerValues.rp + " target lp: " + attackEventMessage.targetValues.lp + " rp: " + attackEventMessage.targetValues.rp);


                    if (attackEventMessage.player == playerNumber)
                    {
                        yourMonsterZone.UpdateCardData(true, attackEventMessage.attackerValues.index, attackEventMessage.attackerValues.lp, attackEventMessage.attackerValues.rp);
                        enemyMonsterZone.UpdateCardData(false, attackEventMessage.targetValues.index, attackEventMessage.targetValues.lp, attackEventMessage.targetValues.rp);
                        if (attackEventMessage.attackerValues.lp <= 0 || attackEventMessage.attackerValues.rp <= 0) yourMonsterZone.RemoveMonsterCard(attackEventMessage.attackerValues.index);
                        if (attackEventMessage.targetValues.lp <= 0 || attackEventMessage.targetValues.rp <= 0) enemyMonsterZone.RemoveEnemyMonsterCard(attackEventMessage.targetValues.index);
                        
                    }
                    else
                    {
                        enemyMonsterZone.UpdateCardData(false, attackEventMessage.attackerValues.index, attackEventMessage.attackerValues.lp, attackEventMessage.attackerValues.rp);
                        yourMonsterZone.UpdateCardData(true, attackEventMessage.targetValues.index, attackEventMessage.targetValues.lp, attackEventMessage.targetValues.rp);
                        if (attackEventMessage.attackerValues.lp <= 0 || attackEventMessage.attackerValues.rp <= 0) enemyMonsterZone.RemoveEnemyMonsterCard(attackEventMessage.attackerValues.index);
                        if (attackEventMessage.targetValues.lp <= 0 || attackEventMessage.targetValues.rp <= 0) yourMonsterZone.RemoveMonsterCard(attackEventMessage.targetValues.index);
                    }

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
    }

    [Button] public async void OpenNewConnection()
    {
        
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
            await websocket.SendText(message);
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    [Button] public async void CloseConnection()
    {
        await websocket.Close();
    }

    [Button]
    public static void PlayCard(int cardIndex, int boardIndex)
    {
        Debug.Log("Playing card to index " + boardIndex + " from " + cardIndex);

        PlayCardMessage playCardMessage = new PlayCardMessage(1, cardIndex, boardIndex);
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

    [Button]public static void Attack(int fieldIndex)
    {
        Debug.Log("Attacked with id " + fieldIndex);

        GameMessage message = new GameMessage("OnMessage", "ATTACK", fieldIndex.ToString());
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
}
