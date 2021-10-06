using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerNumber;
    public static GameManager Instance { get; private set; }

    [SerializeField] public MonsterZone yourMonsterZone;
    [SerializeField] public MonsterZone enemyMonsterZone;
    public PlayerStats playerStats;
    public PlayerStats enemyPlayerStats;

    [SerializeField] private int playerStartHealth = 100;

    private static GameObject sfxLibrary;

    private void Awake()
    {
        Instance = gameObject.GetComponent<GameManager>();
        sfxLibrary = GameObject.Find("SFXLibrary");
    }

    private void Start()
    {
        playerStats = new PlayerStats(playerStartHealth);
        enemyPlayerStats = new PlayerStats(playerStartHealth);
    }


    //Message from server is directed trough this code before  the actual function
    public void PlayerBurnCard(BurnCardMessage burnCardMessage)
    {

        burnCardMessage.burnedCardDone = JsonUtility.FromJson<DrawCardMessage>(burnCardMessage.burnedCard);
        DrawCardMessage cardMessage = burnCardMessage.burnedCardDone;
        if (cardMessage.player == playerNumber)
        {
            
        }
        else
        {
            GameObject newCard;
            newCard = Instantiate(References.i.handCard, References.i.opponentBonfire.transform.position, Quaternion.Euler(0, 180, 0));
            Hand.Instance.RemoveCard(burnCardMessage.handIndex);
            CardData cardData = References.i.cardList.GetCardData(cardMessage);
            newCard.GetComponent<InGameCard>().cardData = cardData;
            PlayerBurnCard(cardMessage.player, newCard);
        }
        
    }


    //Trigger sound effect and all that stuff
    //Called from server for enemy player and from Mouse script for client owener
    public void PlayerBurnCard(int player, GameObject card)
    {
        


        sfxLibrary.GetComponent<BurnSFX>().Play();
        card.GetComponent<InGameCard>().Burn();
        int value = card.GetComponent<InGameCard>().cardData.value;
        if (player == playerNumber)
        {
            Debug.LogWarning("START YOUR BURN EFFECT HERE FOR THE CARD");
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            Debug.LogWarning("START YOUR BURN EFFECT HERE FOR THE CARD");
            card.transform.SetParent(References.i.opponentBonfire.transform);
            UpdatePlayerBurnValue(player, enemyPlayerStats.playerBurnValue + value);
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
    }

    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player ==playerNumber)
        {
            playerStats.playerBurnValue = newValue;
        }
        else
        {
            enemyPlayerStats.playerBurnValue = newValue;
        }
    }
}
