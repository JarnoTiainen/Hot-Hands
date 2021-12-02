using UnityEngine;

public class NonstaticRayCaster : MonoBehaviour
{
    [SerializeField] private Camera rayCamera;
    public GameObject target;
    public GameObject newTarget;
    public GameObject previousTarget;
    public bool firstTimeCall = true;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (target)
            {
                newTarget = hit.transform.gameObject;
                if (target != hit.transform.gameObject)
                {
                    NewTarget(target, hit.transform.gameObject);
                }
            }
            target = hit.transform.gameObject;


            if (Input.GetMouseButtonDown(0))
            {
                if (target.GetComponent<IOnClickDownUIElement>() != null)
                {
                    IOnClickDownUIElement[] iOnClickDownUIElements = target.GetComponents<IOnClickDownUIElement>();
                    foreach (IOnClickDownUIElement element in iOnClickDownUIElements)
                    {
                        element.OnClickElement();
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (target.GetComponent<IOnHoldDownElement>() != null)
                {
                    IOnHoldDownElement[] onHoldDownElements = target.GetComponents<IOnHoldDownElement>();
                    foreach (IOnHoldDownElement element in onHoldDownElements)
                    {
                        element.OnHoldDownElement();
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (target.GetComponent<IOnClickUpElement>() != null)
                {
                    IOnClickUpElement[] onClickUpElements = target.GetComponents<IOnClickUpElement>();
                    foreach (IOnClickUpElement element in onClickUpElements)
                    {
                        element.OnClickUpElement();
                    }
                }
            }

            if (target.GetComponent<IOnHoverUIElement>() != null)
            {
                IOnHoverUIElement[] onHoverUIElements = target.GetComponents<IOnHoverUIElement>();
                foreach (IOnHoverUIElement element in onHoverUIElements)
                {
                    element.OnHoverElement();
                }
            }
        }
        else
        {
            if (!previousTarget && target)
            {
                previousTarget = target;
                target = null;
            }
            if (previousTarget)
            {
                if (previousTarget.GetComponent<IOnHoverExitElement>() != null)
                {
                    IOnHoverExitElement[] onHoverExitElements = previousTarget.GetComponents<IOnHoverExitElement>();
                    foreach (IOnHoverExitElement element in onHoverExitElements)
                    {
                        element.OnHoverExit();
                    }
                }
                previousTarget = null;
            }
        }
    }

    private void NewTarget(GameObject oldTarget, GameObject newTarget)
    {
        if (oldTarget)
        {
            if (oldTarget.GetComponent<IOnHoverExitElement>() != null)
            {
                IOnHoverExitElement[] onHoverExitElements = oldTarget.GetComponents<IOnHoverExitElement>();
                foreach (IOnHoverExitElement element in onHoverExitElements)
                {
                    element.OnHoverExit();
                }
            }
        }

        if (newTarget.GetComponent<IOnHoverEnterElement>() != null)
        {
            IOnHoverEnterElement[] onHoverEnterElements = newTarget.GetComponents<IOnHoverEnterElement>();
            foreach (IOnHoverEnterElement element in onHoverEnterElements)
            {
                element.OnHoverEnter();
            }
        }

    }
}

