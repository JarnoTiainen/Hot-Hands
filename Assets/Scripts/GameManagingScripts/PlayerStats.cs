using System;
[Serializable]
public class PlayerStats
{
    public int deckCardCount;
    public int discardpileCardCount;
    public int playerBurnValue;
    public int playerHealth;
    public int playerHandCards;
    public int playerFieldCards;

    public PlayerStats(int playerStartHealth)
    {
        deckCardCount = 0;
        discardpileCardCount = 0;
        playerBurnValue = 0;
        playerHealth = playerStartHealth;
    }
}
