using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellMeshCreator : MonoBehaviour
{
    [SerializeField] GameObject topRight;
    [SerializeField] GameObject topLeft;
    [SerializeField] GameObject middleRight;
    [SerializeField] GameObject middleLeft;
    [SerializeField] GameObject BottomRight;
    [SerializeField] GameObject BottomLeft;

    [SerializeField] MeshFilter meshFilter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button] public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] verticies = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];

        verticies[0] = new Vector3(BottomLeft.transform.localPosition.x, BottomLeft.transform.localPosition.y);
        verticies[1] = new Vector3(middleLeft.transform.localPosition.x, middleLeft.transform.localPosition.y);
        verticies[2] = new Vector3(topLeft.transform.localPosition.x, topLeft.transform.localPosition.y);
        //verticies[3] = new Vector3(topRight.transform.localPosition.x, topRight.transform.localPosition.y);
        //verticies[4] = new Vector3(middleRight.transform.localPosition.x, middleRight.transform.localPosition.y);
        //verticies[5] = new Vector3(BottomRight.transform.localPosition.x, BottomRight.transform.localPosition.y);

        uv[0] = new Vector3(BottomLeft.transform.localPosition.x, BottomLeft.transform.localPosition.y);
        uv[1] = new Vector3(middleLeft.transform.localPosition.x, middleLeft.transform.localPosition.y);
        uv[2] = new Vector3(topLeft.transform.localPosition.x, topLeft.transform.localPosition.y);
        //uv[3] = new Vector3(topRight.transform.localPosition.x, topRight.transform.localPosition.y);
        //uv[4] = new Vector3(middleRight.transform.localPosition.x, middleRight.transform.localPosition.y);
        //uv[5] = new Vector3(BottomRight.transform.localPosition.x, BottomRight.transform.localPosition.y);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        /*
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 5;
        triangles[6] = 5;
        triangles[7] = 2;
        triangles[8] = 3;
        triangles[9] = 5;
        triangles[10] = 3;
        triangles[11] = 4;
        */

        meshFilter.mesh = mesh;
    }
}
