using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LineRendererManager : MonoBehaviour
{
    public static LineRendererManager Instance { get; private set; }


    [SerializeField] private float scale;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private GameObject lineGameObject;
    [SerializeField] private AnimationCurve lineThickness;
    [SerializeField] private int numberOfVerticies;
    Mesh mesh;




    private void Awake()
    {
        Instance = gameObject.GetComponent<LineRendererManager>();
    }

    private void Start()
    {
        mesh = CreateMesh();
    }

    [Button] public void CreateNewLine(GameObject sourceGameObject, GameObject targetGameObject)
    {
        Line newLineGameObject = Instantiate(lineGameObject).GetComponent<Line>();
        newLineGameObject.SetNewTargetAndSource(targetGameObject, sourceGameObject, scale, lineMaterial.shader, mesh);
    }

    [Button]
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        int numberOfVerticies = 100;
        Vector3[] verticies = new Vector3[numberOfVerticies * 3];
        Vector2[] uv = new Vector2[numberOfVerticies * 3];
        int[] triangles = new int[numberOfVerticies * 12 - 12];
        float gapBetweenVerticies = 1f / (numberOfVerticies - 1);



        //set verticies
        for (int i = 0; i < numberOfVerticies; i++)
        {
            verticies[i + numberOfVerticies] = new Vector3(-0.5f + gapBetweenVerticies * i, lineThickness.Evaluate(gapBetweenVerticies * i) / 2);
            verticies[i] = new Vector3(-0.5f + gapBetweenVerticies * i, 0f);
            verticies[i + numberOfVerticies * 2] = new Vector3(-0.5f + gapBetweenVerticies * i, -lineThickness.Evaluate(gapBetweenVerticies * i) / 2);

            uv[i + numberOfVerticies] = new Vector3(0 + gapBetweenVerticies * i, 1);
            uv[i] = new Vector3(0 + gapBetweenVerticies * i, 0.5f);
            uv[i + numberOfVerticies * 2] = new Vector3(0 + gapBetweenVerticies * i, 0);
        }

        //set triangles
        for (int i = 0; i < numberOfVerticies - 1; i++)
        {
            triangles[0 + 12 * i] = i;
            triangles[1 + 12 * i] = i + numberOfVerticies;
            triangles[2 + 12 * i] = i + 1;

            triangles[3 + 12 * i] = i + 1;
            triangles[4 + 12 * i] = i + numberOfVerticies;
            triangles[5 + 12 * i] = i + 1 + numberOfVerticies;

            triangles[6 + 12 * i] = i;
            triangles[7 + 12 * i] = i + 1;
            triangles[8 + 12 * i] = i + numberOfVerticies * 2;

            triangles[9 + 12 * i] = i + 1;
            triangles[10 + 12 * i] = i + numberOfVerticies * 2 + 1;
            triangles[11 + 12 * i] = i + numberOfVerticies * 2;
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }


}
