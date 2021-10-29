using UnityEngine;
using NativeWebSocket;
using Sirenix.OdinInspector;
using SimpleJSON;
public class WebSocketService : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    
    public static WebSocketService Instance { get; private set; }
    static WebSocket websocket;
    [SerializeField] private bool debuggerModeOn = false;
    [SerializeField] private bool showServerMessage = false;

    public bool isLoggedIn = false;
    

    private void Awake()
    {
        Instance = gameObject.GetComponent<WebSocketService>();
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
       
        websocket = new WebSocket("wss://n14bom45md.execute-api.eu-north-1.amazonaws.com/production");
        OpenNewConnection();

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
            if (showServerMessage) Debug.Log("server message: " + data[0] + " " + data[1]);
            switch ((string)data[0])
            {
                case "OPPONENTJOIN":
                    if (debuggerModeOn) Debug.Log("Message type was OPPONENTJOINED");
                    Debug.Log("Opponent joined!");
                    break;
                case "GETSIDE":
                    if (debuggerModeOn) Debug.Log("Message type was GETSIDE");
                    gameManager.SetPlayerNumber(int.Parse(data[1]));
                    break;
                case "SUMMONCARD":
                    if (debuggerModeOn) Debug.Log("Message type was SUMMONCARD");
                    SummonCardMessage summonCardMessage = JsonUtility.FromJson<SummonCardMessage>(data[1]);
                    gameManager.PlayerSummonCard(summonCardMessage);
                    break;
                case "SUMMONDENIED":
                    gameManager.CardSummonDenied();
                    break;
                case "REMOVECARD":
                    RemoveCardMessage removeCardMessage = JsonUtility.FromJson<RemoveCardMessage>(data[1]);
                    gameManager.RemoveCard(removeCardMessage);
                    break;
                case "PLAYCARD":
                    if (debuggerModeOn) Debug.Log("Message type was PLAYCARD");
                    PlayCardMessage playCardMessage = JsonUtility.FromJson<PlayCardMessage>(data[1]);
                    gameManager.PlayerPlayCard(playCardMessage);
                    break;
                case "DRAWCARD":
                    if (debuggerModeOn) Debug.Log("Message type was DRAWCARD");
                    if(data[1] != "DENIED")
                    {
                        DrawCardMessage drawCardMessage = JsonUtility.FromJson<DrawCardMessage>(data[1]);
                        gameManager.PlayerDrawCard(drawCardMessage);
                    }
                    else
                    {
                        Debug.LogWarning("CARD DRAW FAILED DESTROYING HIDDEN CARD");
                        gameManager.PlayerReturnDrawCard();
                    }
                    break;
                case "ATTACK":

                    //Add attack denied by server
                    if (debuggerModeOn) Debug.Log("Message type was ATTACK");
                    AttackEventMessage attackEventMessage = JsonUtility.FromJson<AttackEventMessage>(data[1]);
                    gameManager.PlayerAttack(attackEventMessage);


                    break;
                case "SAVECARD":
                    if (debuggerModeOn) Debug.Log("Message type was SAVECARD");
                    Debug.Log("Saved card " + data[1][0] + " succesfully");
                    break;
                case "SETDECK":
                    if (debuggerModeOn) Debug.Log("Message type was SETDECK");
                    SetDeckMessage setDeckMessage = JsonUtility.FromJson<SetDeckMessage>(data[1]);
                    GameManager.Instance.SetDeck(setDeckMessage);
                    break;
                case "BURNCARD":
                    if (debuggerModeOn) Debug.Log("Message type was BURNCARD");
                    BurnCardMessage burnCardMessage = JsonUtility.FromJson<BurnCardMessage>(data[1]);
                    gameManager.PlayerBurnCard(burnCardMessage);
                    break;
                case "SERVERCLIENTMISSMATCH":
                    Debug.LogError("MISS MATCH BETWEEN SERVER AND CLIENT! " + data[1]);
                    break;
                case "ENCHANTMENTACTIVE":
                    if (debuggerModeOn) Debug.Log("Message type was ENCHANTMENTACTIVE");
                    EnchantmentEffectMesage enchantmentEffect = JsonUtility.FromJson<EnchantmentEffectMesage>(data[1]);
                    gameManager.EnchantmentEffectActive(enchantmentEffect);
                    break;
                case "SAVEUSER":
                    if(data[1] == "ok")
                    {
                        isLoggedIn = true;
                        Debug.Log("Created new account");
                    }
                    else
                    {
                        Debug.Log("Created new account Failed!");
                    }
                    break;
                case "LOGIN":
                    if (data[1] == "ok")
                    {
                        isLoggedIn = true;
                        GameObject.Find("LoginPanel").GetComponent<LoginManager>().CloseLoginScreen();
                        Debug.Log("Login ok");
                        
                    }
                    else
                    {
                        Debug.Log("Login Failed!");
                    }
                    break;
                case "JOINGAME":
                    if (data[1] == "ok")
                    {
                        Debug.Log("MATCH FOUND!");
                        if(GameObject.Find("Canvas").GetComponent<MainMenu>())
                        {
                            GameObject.Find("Canvas").GetComponent<MainMenu>().GameFound(1);
                        }
                        
                        GetPlayerNumber();
                        //References.i.yourDeckObj.GetComponent<Deck>().SendDeckData();
                    }
                    break;
                case "GAINBURN":
                    gameManager.PlayerGainBurnValue(JsonUtility.FromJson<GainBurnValueMessage>(data[1]));
                    break;

                case "ADDNEWCARD":
                    gameManager.AddNewCard(JsonUtility.FromJson<AddNewCardMessage>(data[1]));
                    break;
                default:
                    if (debuggerModeOn) Debug.LogError("MESSAGE WAS UNKOWN: " + data[0] + " " + data[1]);
                    break;
            }
        };
        Debug.Log("opening");
    }

    [Button] public async void OpenNewConnection()
    {
        await websocket.Connect();
    }

    [Button] public static void JoinGame()
    {
        GameMessage message = new GameMessage("OnMessage", "JOINGAME", "");
        SendWebSocketMessage(JsonUtility.ToJson(message));
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

    public async void CloseConnection()
    {
        await websocket.Close();
    }

    public static void PlayCard(int cardIndex, int boardIndex)
    {
        if (Instance.debuggerModeOn) Debug.LogWarning("PLAYING CARD TO INDEX " + boardIndex);
        PlayCardMessage playCardMessage = new PlayCardMessage(2, cardIndex, boardIndex);
        string playCardMessageJSON = JsonUtility.ToJson(playCardMessage);

        GameMessage message = new GameMessage("OnMessage", "PLAYCARD", playCardMessageJSON);

        SendWebSocketMessage(JsonUtility.ToJson(message));
        //sfxLibrary.GetComponent<PlayCardSFX>().Play();
    }


    [Button]
    public static void CreateNewAccount(string name, string password, string email)
    {
        CreateUsersMessage createUserMessage = new CreateUsersMessage(name, password, email);
        GameMessage message = new GameMessage("OnMessage", "SAVEUSER", JsonUtility.ToJson(createUserMessage));
        SendWebSocketMessage(JsonUtility.ToJson(message));
        
    }
    [Button]
    public static void Login(string name, string password)
    {
        CreateUsersMessage createUserMessage = new CreateUsersMessage(name, password);
        GameMessage message = new GameMessage("OnMessage", "LOGIN", JsonUtility.ToJson(createUserMessage));
        SendWebSocketMessage(JsonUtility.ToJson(message));

    }



    public static void GetPlayerNumber()
    {
        GameMessage playCard = new GameMessage("OnMessage", "GETSIDE", "");
        SendWebSocketMessage(JsonUtility.ToJson(playCard));
    }


    public static void DrawCard()
    {
        GameMessage message = new GameMessage("OnMessage", "DRAWCARD", "");
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    public static void SaveCardToDataBase(Card card)
    {
        Debug.Log("sending new cards");
        GameMessage message = new GameMessage("OnMessage", "SAVECARD", card.CreateCardJSON());
        card.SaveCardToCardList();
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
    public static void SetDeck(string deckJson)
    {
        if(Instance.debuggerModeOn) Debug.Log("Send deck data");

        GameMessage message = new GameMessage("OnMessage", "SETDECK", deckJson);
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    public static void Attack(int fieldIndex)
    {
        Debug.Log("Attacked with id " + fieldIndex);

        GameMessage message = new GameMessage("OnMessage", "ATTACK", fieldIndex.ToString());
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    public static void Burn(int handIndex)
    {
        if (Instance.debuggerModeOn) Debug.Log("Burned card hand index " + handIndex);

        GameMessage message = new GameMessage("OnMessage", "BURNCARD", handIndex.ToString());
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
}
