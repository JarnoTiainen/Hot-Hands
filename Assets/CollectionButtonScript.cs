using UnityEngine;

public class CollectionButtonScript : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private Button button;
    public enum Button
    {
        Save, Clear
    }
    

    public void OnClickElement()
    {
        meshRenderer.material.SetInt("_IsClicked", 1);
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.SetInt("_IsHovered", 1);
    }

    public void OnHoverExit()
    {
        meshRenderer.material.SetInt("_IsHovered", 0);
    }

    private void Start()
    {
        meshRenderer.material = material;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            meshRenderer.material.SetInt("_IsClicked", 0);
        }
    }


}
