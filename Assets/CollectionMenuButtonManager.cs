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
    [SerializeField] private bool activeDeck = false;
    private static GameObject sfxLibrary;

    private enum ButtonType
    {
        All,    //0
        Deck1,  //1
        Deck2,  //2
        Deck3,  //3
        Deck4,  //4
        Deck5   //5
    }

    public void OnClickElement()
    {
        
        if (deckSelected) return;
        ToggleButton((int)buttonType);
        CollectionManager.Instance.ChangeActiveCardList((int)buttonType);
        sfxLibrary.GetComponent<ButtonSFX>().OnClick();
        DeckBuilder.Instance.EditDeck();
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.SetInt("_IsHovered", 1);
    }

    public void OnHoverExit()
    {
        meshRenderer.material.SetInt("_IsHovered", 0);
    }

    private void Awake()
    {
        meshRenderer.material = material;
        sfxLibrary = GameObject.Find("SFXLibrary");
    }

    [Button]
    public void ToggleButton(int index)
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

        int siblings = gameObject.transform.parent.childCount;
        for(int i = 0; siblings > i; i++)
        {
            if(i != index)
            {
                gameObject.transform.parent.GetChild(i).GetComponent<CollectionMenuButtonManager>().TurnOff();
            }
        }
    }

    public void TurnOff()
    {
        deckSelected = false;
        meshRenderer.material.SetInt("_ButtonSelected", 0);
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

    public void ChangeDeckName(string text)
    {
        buttonText.text = text;
    }
}
