using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SinusoidalWave : MonoBehaviour
{
    [Header("Mesh")]
    public int width = 50;        // número de vértices en X
    public int height = 50;       // número de vértices en Z
    public float spacing = 0.5f;  // distancia entre vértices

    [Header("Wave Parameters")]
    public float amplitude = 1f;        // A: altura máxima de la ola
    public float wavelength = 5f;       // L: longitud de onda
    public float frequency = 1f;        // f: frecuencia (ciclos por segundo)
    public float phase = 0f;            // p: desfase inicial
    public Vector2 direction = new Vector2(1f, 0f); // D: dirección de la ola (XZ)

    [Header("UI")]
    public Toggle waveToggle;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] baseVertices; // posiciones originales (sin ola)
    private bool waveActive = true;


    void Start()
    {
        GenerateMesh();

        if (waveToggle != null)
        {
            waveToggle.isOn = waveActive;
            waveToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void OnToggleChanged(bool value)
    {
        waveActive = value;

        if (!waveActive)
            FlattenMesh();
    }

    void FlattenMesh()
    {
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = new Vector3(baseVertices[i].x, 0f, baseVertices[i].z);

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int totalVertices = width * height;
        vertices = new Vector3[totalVertices];
        baseVertices = new Vector3[totalVertices];

        //Vértices
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = z * width + x;
                float px = x * spacing;
                float pz = z * spacing;
                vertices[i] = new Vector3(px, 0f, pz);
                baseVertices[i] = new Vector3(px, 0f, pz); 
            }
        }

        //Triángulos
        // Cada celda del grid = 2 triángulos = 6 índices
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        int t = 0;
        for (int z = 0; z < height - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int i = z * width + x;
                // Triángulo 1
                triangles[t++] = i;
                triangles[t++] = i + width;
                triangles[t++] = i + 1;
                // Triángulo 2
                triangles[t++] = i + 1;
                triangles[t++] = i + width;
                triangles[t++] = i + width + 1;
            }
        }

        //UVs
        Vector2[] uvs = new Vector2[totalVertices];
        for (int z = 0; z < height; z++)
            for (int x = 0; x < width; x++)
                uvs[z * width + x] = new Vector2((float)x / width, (float)z / height);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    void Update()
    {
        if (!waveActive) return;

        float t = Time.time;

        // Velocidad de propagación: v = f * L
        float speed = frequency * wavelength;

        // Normalizamos la dirección
        Vector2 dir = direction.normalized;

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = baseVertices[i].x;
            float z = baseVertices[i].z;

            // Proyectamos el vértice sobre la dirección de la ola
            float projection = dir.x * x + dir.y * z;

            // A * sin((2π/L) * (x - v*t) + p)
            float y = amplitude * Mathf.Sin((2f * Mathf.PI / wavelength) * (projection - speed * t) + phase);

            vertices[i] = new Vector3(x, y, z);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void OnDestroy()
    {
        if (waveToggle != null)
            waveToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}