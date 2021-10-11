using System.Collections;
using System.Collections.Generic;

public class PlayCardMessage
{
    public enum CardSource
    {
        Default,     //0
        Deck,        //1
        Hand,        //2
        DiscardPile, //3
    }
    public bool denied;
    public int cardSource;
    public int handIndex;
    public int boardIndex;

    public int player;
    public float attackCooldown;
    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public int rp;
    public int lp;


    public PlayCardMessage(int cardSource = 0, int handIndex = 0, int boardIndex = 0, int sender = 0)
    {
        this.cardSource = cardSource;
        this.handIndex = handIndex;
        this.player = sender;
        this.boardIndex = boardIndex;
    }
}
