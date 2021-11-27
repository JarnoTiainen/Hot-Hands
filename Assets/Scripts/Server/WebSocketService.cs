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
            if (showServerMessage) Debug.Log("server message: " + data[0] + " " + data[1] + " " + data[2]);
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
                case "GETDECKS":
                    if (debuggerModeOn) Debug.Log("Message type was GETDECKS");
                    GetDecksMessage  getDecksMessage = JsonUtility.FromJson<GetDecksMessage>(data[1]);
                    CollectionManager.Instance.SetPlayerDecks(getDecksMessage);
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
                        Debug.Log("Created new account");
                        MainMenu.Instance.CreatePopupNotification("Created new account.");
                    }
                    else
                    {
                        Debug.Log("Created new account Failed!");
                        MainMenu.Instance.CreatePopupNotification("Account creation failed!");
                    }
                    break;
                case "LOGIN":
                    if (data[1] == "ok")
                    {
                        isLoggedIn = true;
                        GameObject.Find("LoginPanel").GetComponent<LoginManager>().CloseLoginScreen();
                        Debug.Log("Login ok");
                        MainMenu.Instance.CreatePopupNotification("Logged in.");
                        GetDecks();
                    }
                    else
                    {
                        Debug.Log("Login Failed!");
                        MainMenu.Instance.CreatePopupNotification("Login Failed!");
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
                case "MODIFYHEALTH":
                    gameManager.PlayerModifyHealth(JsonUtility.FromJson<ModifyHealth>(data[1]));
                    break;
                case "BUFFBOARD":
                    gameManager.CardDataChange(JsonUtility.FromJson<StatChangeMessage>(data[1]));
                    break;
                case "SEEDMISMATCH":
                    Debug.LogError("SEED MISSMATCH! not found");
                    break;
                case "PLAYSPELL":
                    if (debuggerModeOn) Debug.Log("Message type was PLAYSPELL");
                    gameManager.PlaySpell(JsonUtility.FromJson<PlaySpellMessage>(data[1]));
                    break;
                case "TRIGGERSPELL":
                    gameManager.TriggerSpell(JsonUtility.FromJson<TriggerSpellMessage>(data[1]));
                    break;
                case "LOADCHAT":
                    gameManager.LoadChat(JsonUtility.FromJson<LoadChatMessage>(data[1]));
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

    [Button] public static void JoinGame(bool soloPlayEnabled)
    {
        string msg = "";
        if (soloPlayEnabled) msg = "solo";
        GameMessage message = new GameMessage("OnMessage", "JOINGAME", msg);
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

    public static void PlayCard(int boardIndex, string seed, string targetSeed = "")
    {
        Debug.Log("playing card with seed: " + seed);
        if (Instance.debuggerModeOn) Debug.LogWarning("PLAYING CARD TO INDEX " + boardIndex);
        PlayCardMessage playCardMessage = new PlayCardMessage(seed, 2, boardIndex, targetSeed);
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
    [Button]
    public static void GetDecks()
    {
        GameMessage message = new GameMessage("OnMessage", "GETDECKS", "");
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
    public static void SetActiveDeck(int deckIndex)
    {
        if(Instance.debuggerModeOn) Debug.Log("Send deck data");

        GameMessage message = new GameMessage("OnMessage", "SETACTIVEDECK", deckIndex.ToString());
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
    public static void SaveDeck(string deckJson)
    {
        if (Instance.debuggerModeOn) Debug.Log("Send deck data " + deckJson);

        GameMessage message = new GameMessage("OnMessage", "SAVEDECK", deckJson);
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    //Make this main function and delete one above
    public static void Attack(string seed)
    {
        Debug.Log("Attacked with seed " + seed);

        GameMessage message = new GameMessage("OnMessage", "ATTACK", seed);
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

    //Make this main function and delete one above
    public static void Burn(string seed)
    {
        Debug.Log("burnign card with: " + seed);
        if (Instance.debuggerModeOn) Debug.Log("Burned card with seed " + seed);

        GameMessage message = new GameMessage("OnMessage", "BURNCARD", seed);
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }
    public static void TriggerSpellChain()
    {
        Debug.Log("Sending message trigger spellchain");
        GameMessage message = new GameMessage("OnMessage", "TRIGGERSPELLCHAIN", "");
        SendWebSocketMessage(JsonUtility.ToJson(message));
    }

}
