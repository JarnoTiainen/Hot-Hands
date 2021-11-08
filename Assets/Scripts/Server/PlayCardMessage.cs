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
    public bool auto;
    public int serverBurnValue;
    public int cardSource;
    public int boardIndex;
    public string targetSeed;

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
    public List<Enchantment> enchantments;
    public string seed;


    public PlayCardMessage(string seed, int cardSource = 0, int boardIndex = 0, string targetSeed = "")
    {
        this.seed = seed;
        this.cardSource = cardSource;
        this.boardIndex = boardIndex;
        this.targetSeed = targetSeed;
    }
}
