using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CollectionMenuButtonManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private bool deckSelected = false;
    [SerializeField] private bool activeDeck = false;

    private enum ButtonType
    {
        Deck1,
        Deck2,
        Deck3,
        Deck4,
        Deck5
    }

    public void OnClickElement()
    {
        Debug.Log("Clicked button: " + gameObject.name);
        switch (buttonType)
        {
            case ButtonType.Deck1:
                break;
            case ButtonType.Deck2:
                break;
            case ButtonType.Deck3:
                break;
            case ButtonType.Deck4:
                break;
            case ButtonType.Deck5:
                break;
            default:
                break;
        }

    }

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }

    private void Awake()
    {
        meshRenderer.material = material;
    }

    [Button]
    public void ToggleButton()
    {
        if (!deckSelected)
        {
            deckSelected = true;
            meshRenderer.material.SetInt("_ButtonSelected", 1);
        }
        else
        {
            deckSelected = false;
            meshRenderer.material.SetInt("_ButtonSelected", 0);
        }
    }


    [Button]
    public void ToggleDeckSelected()
    {
        if (!activeDeck)
        {
            activeDeck = true;
            meshRenderer.material.SetInt("_DeckSelected", 1);
        }
        else
        {
            activeDeck = false;
            meshRenderer.material.SetInt("_DeckSelected", 0);
        }
    }
}
