using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Line : MonoBehaviour
{
    [SerializeField] private GameObject source;
    [SerializeField] private GameObject target;
    [SerializeField] MeshRenderer textureMesh;
    [SerializeField] private float scale;
    private int numberOfVerticies;
    bool isLineActive;

    // Update is called once per frame

    private void FixedUpdate()
    {
        if (source && target)
        {
            SetLineTransform();
        }
        else
        {
            RemoveLine();
        }
    }

    public void SetLineTransform()
    {
        Vector3 endPos = source.transform.position;
        Vector3 startPos = target.transform.position;

        //position
        transform.GetChild(0).position = (endPos + startPos) / 2;

        //rotation
        Vector3 targetDir = startPos - endPos;
        float angleXY = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        float hyp = Mathf.Sqrt(Mathf.Pow(targetDir.y, 2) + Mathf.Pow(targetDir.x, 2));
        float angleZhyp = Mathf.Atan2(targetDir.z, hyp) * Mathf.Rad2Deg;
        var quatXY = Quaternion.AngleAxis(angleXY, Vector3.forward);
        var quatZX = Quaternion.AngleAxis(angleZhyp, Vector3.down);


        transform.GetChild(0).rotation = quatXY * quatZX;
        //scale
        float lineLength = Vector3.Distance(endPos, startPos);
        transform.GetChild(0).GetChild(0).localScale = new Vector3(lineLength, scale, scale);

        if (isLineActive) SetLineActive();
        isLineActive = true;
    }



    public void SetNewTargetAndSource(GameObject target, GameObject source, float scale, Shader newLineShader, Mesh  mesh)
    {
        textureMesh.material.shader = newLineShader;
        this.scale = scale;
        textureMesh.material.SetFloat("_TextureScale", this.scale);
        this.target = target;
        this.source = source;

        transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh = mesh;
        SetLineTransform();
    }

    [Button] public void SetLineActive()
    {
        transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
    }

    public void RemoveLine()
    {
        Destroy(gameObject);
    }
}
