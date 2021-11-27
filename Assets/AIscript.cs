using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIscript : MonoBehaviour
{
    bool doOnce = true;
    int drawnCards = 0;

    private void Update()
    {
        if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.CardDraw && drawnCards == 0) {
            References.i.opponentDeck.GetComponent<TutorialDeck>().OpponentDraw();
            doOnce = false;
            drawnCards++;
        }
        

    }
}
