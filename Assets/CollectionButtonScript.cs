using UnityEngine;
using System.Collections;

public class CollectionButtonScript : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    public MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private Button button;
    public enum Button
    {
        Save,
        Clear,
        OpenRename,
        SaveRename,
        CloseRename,
        SetActive,
        Back
    }

    private void Start()
    {
        meshRenderer.material = material;
    }

    private void OnDisable()
    {
        meshRenderer.material.SetInt("_IsClicked", 0);
        StopAllCoroutines();
    }

    public void OnClickElement()
    {
        StartCoroutine(ClickAnimation());
        switch (button)
        {
            case Button.Save:
                DeckBuilder.Instance.SaveDeck();
                break;
            case Button.Clear:
                DeckBuilder.Instance.ClearBuild();
                break;
            case Button.OpenRename:
                CollectionManager.Instance.RenamePopupSetActive(true);
                break;
            case Button.SaveRename:
                CollectionManager.Instance.SaveDeckToDB(-1, true);
                break;
            case Button.CloseRename:
                CollectionManager.Instance.RenamePopupSetActive(false);
                break;
            case Button.SetActive:
                CollectionManager.Instance.SetActiveDeckToDB();
                break;
            case Button.Back:
                MainMenu.Instance.CollectionMenuSetActive(false);
                break;
            default:
                break;
        }
    }

    private IEnumerator ClickAnimation()
    {
        meshRenderer.material.SetInt("_IsClicked", 1);
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }
        meshRenderer.material.SetInt("_IsClicked", 0);
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.SetInt("_IsHovered", 1);
    }

    public void OnHoverExit()
    {
        meshRenderer.material.SetInt("_IsHovered", 0);
    }
}
