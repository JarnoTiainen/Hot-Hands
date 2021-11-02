using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class line : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private GameObject source;
    [SerializeField] private GameObject target;
    [SerializeField] private float lineLength;
    [SerializeField] private float tileSize;
    [SerializeField] private GameObject lineGameObject;
    [SerializeField] private Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        lineMaterial = lineGameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(source && target)
        {
            startPos = source.transform.position;
            endPos = target.transform.position;
            lineLength = Vector3.Distance(startPos, endPos);
            Vector3 lineOBJPos = (startPos + endPos) / 2;
            lineGameObject.transform.position = lineOBJPos;
            Debug.DrawLine(startPos, endPos);
            lineGameObject.transform.LookAt(endPos, Vector3.up);
            lineGameObject.transform.GetChild(0).localScale = new Vector3(0.1f, lineLength, 0.1f);
            float rot = Vector3.Angle(Vector3.up, endPos - startPos);
            if ((endPos - startPos).x > 0)
            {
                rot = Vector3.Angle(Vector3.up, - endPos + startPos);
            }
            lineMaterial.SetFloat("_Rotation", rot);
        }
        
    }


    [Button] public void SetNewSource(GameObject newSource)
    {
        source = newSource;
    }
    [Button] public void SetNewTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void SetNewPosition(Vector2 newStartPos, Vector2 newEndPos)
    {
        startPos = newStartPos;
        endPos = newEndPos;
    }
}
