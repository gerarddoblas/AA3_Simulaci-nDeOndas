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
    public float amplitude = 1f;        // A: amplitud
    public float wavelength = 5f;       // L: longitud de onda
    public float frequency = 1f;        // f: frecuencia
    public float phase = 0f;            // fase inicial
    public Vector2 direction = new Vector2(1f, 0f); // dirección en XZ

    [Header("UI")]
    public Toggle waveToggle;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] baseVertices;

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

    void Update()
    {
        if (!IsWaveActive())
        {
            FlattenMesh();
            return;
        }

        float t = Time.time;

        // v = f * L
        float speed = frequency * wavelength;

        // Normalización manual de la dirección
        float directionMagnitude = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);

        float dirX = 1f;
        float dirZ = 0f;

        if (directionMagnitude > 0f)
        {
            dirX = direction.x / directionMagnitude;
            dirZ = direction.y / directionMagnitude;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = baseVertices[i].x;
            float z = baseVertices[i].z;

            // Proyección manual sobre la dirección de la ola
            float projection = dirX * x + dirZ * z;

            // y = A * sin((2π / L) * (x - v * t) + phase)
            float y = amplitude * Mathf.Sin(
                (2f * Mathf.PI / wavelength) * (projection - speed * t) + phase
            );

            vertices[i] = new Vector3(x, y, z);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "SinusoidalWaterMesh";

        GetComponent<MeshFilter>().mesh = mesh;

        int totalVertices = width * height;

        vertices = new Vector3[totalVertices];
        baseVertices = new Vector3[totalVertices];

        float halfWidth = (width - 1) * spacing * 0.5f;
        float halfHeight = (height - 1) * spacing * 0.5f;

        // Vértices centrados en el origen
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = z * width + x;

                float px = x * spacing - halfWidth;
                float pz = z * spacing - halfHeight;

                vertices[i] = new Vector3(px, 0f, pz);
                baseVertices[i] = new Vector3(px, 0f, pz);
            }
        }

        // Triángulos
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        int triangleIndex = 0;

        for (int z = 0; z < height - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int i = z * width + x;

                // Triángulo 1
                triangles[triangleIndex++] = i;
                triangles[triangleIndex++] = i + width;
                triangles[triangleIndex++] = i + 1;

                // Triángulo 2
                triangles[triangleIndex++] = i + 1;
                triangles[triangleIndex++] = i + width;
                triangles[triangleIndex++] = i + width + 1;
            }
        }

        // UVs
        Vector2[] uvs = new Vector2[totalVertices];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = z * width + x;
                uvs[i] = new Vector2((float)x / width, (float)z / height);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void OnToggleChanged(bool value)
    {
        waveActive = value;

        if (!waveActive)
        {
            FlattenMesh();
        }
    }

    public bool IsWaveActive()
    {
        if (waveToggle != null)
        {
            return waveToggle.isOn;
        }

        return waveActive;
    }

    void FlattenMesh()
    {
        if (mesh == null || baseVertices == null || vertices == null) return;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(baseVertices[i].x, 0f, baseVertices[i].z);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public float GetWaveHeight(float worldX, float worldZ)
    {
        // Si la ola está apagada, el agua está plana.
        if (!IsWaveActive())
        {
            return transform.position.y;
        }

        float t = Time.time;

        // v = f * L
        float speed = frequency * wavelength;

        // Convertimos posición de mundo a posición local del agua
        float localX = worldX - transform.position.x;
        float localZ = worldZ - transform.position.z;

        // Normalización manual de la dirección
        float directionMagnitude = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);

        float dirX = 1f;
        float dirZ = 0f;

        if (directionMagnitude > 0f)
        {
            dirX = direction.x / directionMagnitude;
            dirZ = direction.y / directionMagnitude;
        }

        // Proyección manual
        float projection = dirX * localX + dirZ * localZ;

        // y = A * sin((2π / L) * (x - v * t) + phase)
        float heightValue = amplitude * Mathf.Sin(
            (2f * Mathf.PI / wavelength) * (projection - speed * t) + phase
        );

        return transform.position.y + heightValue;
    }

    void OnDestroy()
    {
        if (waveToggle != null)
        {
            waveToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}