using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class CollectionMenuButtonManager : MonoBehaviour, IOnClickDownUIElement, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private TextMeshProUGUI buttonText;
    public bool deckSelected = false;
    public bool activeDeck = false;

    private enum ButtonType
    {
        Deck1,  //0
        Deck2,  //1
        Deck3,  //2
        Deck4,  //3
        Deck5   //4
    }

    public void OnClickElement()
    {
        if (deckSelected) return;
        ToggleButton();
        SFXLibrary.Instance.buttonClick.PlaySFX();
        DeckBuilder.Instance.OpenDeck((int)buttonType);
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.SetInt("_IsHovered", 1);
        SFXLibrary.Instance.buttonHover.PlaySFX();
    }

    public void OnHoverExit()
    {
        meshRenderer.material.SetInt("_IsHovered", 0);
    }

    private void Awake()
    {
        meshRenderer.material = material;
    }

    [Button]
    public void ToggleButton()
    {
        int siblings = gameObject.transform.parent.childCount;
        for (int i = 0; siblings > i; i++)
        {
            CollectionMenuButtonManager sibling = gameObject.transform.parent.GetChild(i).GetComponent<CollectionMenuButtonManager>();
            if (sibling.deckSelected)
            {
                sibling.TurnOff();
                break;
            }
        }
        deckSelected = true;
        meshRenderer.material.SetInt("_ButtonSelected", 1);
    }

    public void TurnOff()
    {
        deckSelected = false;
        meshRenderer.material.SetInt("_ButtonSelected", 0);
    }

    public void TurnActiveOff()
    {
        activeDeck = false;
        meshRenderer.material.SetInt("_DeckSelected", 0);
    }

    [Button]
    public void ToggleDeckSelected()
    {
        int siblings = gameObject.transform.parent.childCount;
        for (int i = 0; siblings > i; i++)
        {
            CollectionMenuButtonManager sibling = gameObject.transform.parent.GetChild(i).GetComponent<CollectionMenuButtonManager>();
            if (sibling.activeDeck)
            {
                sibling.TurnActiveOff();
                break;
            }
        }
        activeDeck = true;
        meshRenderer.material.SetInt("_DeckSelected", 1);
    }

    public void ChangeDeckName(string text)
    {
        buttonText.text = text;
    }
}
