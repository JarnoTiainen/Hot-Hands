using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionButtons : MonoBehaviour
{
   
    public void SaveDeck()
    {
        GameObject.FindGameObjectWithTag("DeckBuild").GetComponent<DeckBuild>().SaveDeck();
    }


    public void ResetDeck()
    {
        GameObject.FindGameObjectWithTag("DeckBuild").GetComponent<DeckBuild>().ResetDeck();
    }
}
