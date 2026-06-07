using UnityEngine;
using UnityEngine.UI;

public class GerstnerWaves : MonoBehaviour
{

    [Header("Variables")]
    public int resolution = 100;
    public float size = 100;

    [System.Serializable]
    public struct WavesData
    {
        public float _amplitude;
        public float _waveLength;
        public Vector2 _direction;
        public float _speed;
        public float _phaseInitial;
        public float _steepness;
    };


    [Header("Waves Parameters")]
    public WavesData[] waves;

    private Mesh mesh;
    private Vector3[] _baseVertex;
    private Vector3[] _currentVertex;
    private int[] _triangles;

    [Header("UI")]
    public Toggle waveToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMesh();

        if (waveToggle != null)
        {
            waveToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

   
    // Update is called once per frame
    void Update()
    {

        if (mesh == null || _baseVertex == null) return;

        bool active = true;

        if (waveToggle != null)
        {
            active = waveToggle.isOn;
        }

        if (!active)
        {
            FlattenMesh();
            return;
        }

        float t = Time.time;

        for (int i = 0; i < _baseVertex.Length; i++)
        {
            Vector3 _pos = _baseVertex[i];
            Vector3 _displacement = Vector3.zero;

            foreach (var wave in waves)
            {
                _displacement += CalculateGerstner(wave, _pos.x, _pos.z, t);
            }

            _currentVertex[i] = _pos + _displacement;
        }

        mesh.vertices = _currentVertex;
        mesh.RecalculateNormals();
    }

    void OnToggleChanged(bool value)
    {
        Debug.Log("Gerstner toggle value: " + value);
        if (!value)
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

        return true;
    }

    void FlattenMesh()
    {
        if (mesh == null || _baseVertex == null) return;

        mesh.vertices = _baseVertex;
        mesh.RecalculateNormals();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "GerstnerWaterMesh";

        MeshFilter _mf = GetComponent<MeshFilter>();
        if (_mf == null) _mf = gameObject.AddComponent<MeshFilter>();
        _mf.mesh = mesh;

        int vertCount = (resolution + 1) * (resolution + 1);
        _baseVertex = new Vector3[vertCount];
        _currentVertex = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        //Se llenan los vertices
        int idx = 0;
        for (int z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float px = ((float)x / resolution - 0.5f) * size;
                float pz = ((float)z / resolution - 0.5f) * size;
                _baseVertex[idx] = new Vector3(px, 0f, pz);
                uvs[idx] = new Vector2((float)x / resolution, (float)z / resolution);
                idx++;
            }
        }

        //Creo los triangulos
        _triangles = new int[resolution * resolution * 6];
        int t = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = z * (resolution + 1) + x;
                _triangles[t++] = i;
                _triangles[t++] = i + resolution + 1;
                _triangles[t++] = i + 1;
                _triangles[t++] = i + 1;
                _triangles[t++] = i + resolution + 1;
                _triangles[t++] = i + resolution + 2;
            }
        }

        mesh.vertices = _baseVertex;
        mesh.triangles = _triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
    
    Vector3 CalculateGerstner(WavesData wave, float x, float y, float time)
    {
        if (wave._amplitude <= 0f || wave._waveLength <= 0f)
            return Vector3.zero;

        Vector2 _dir = wave._direction.normalized;

        float k = 2f * Mathf.PI / wave._waveLength;
        float dot = _dir.x * x + _dir.y * y;//D·P
        float _totalPhase = k * dot - wave._speed * time + wave._phaseInitial;//fase total

        float cosP = Mathf.Cos(_totalPhase);
        float sinP = Mathf.Sin(_totalPhase);

        //Desplazamineto horitzontal i vertical
        float dx = wave._steepness * wave._amplitude * _dir.x * cosP;
        float dz = wave._steepness * wave._amplitude * _dir.y * cosP;
        float dy = wave._amplitude * sinP;

        return new Vector3(dx, dy, dz);
    }
    public float GetWaveHeight(float worldX, float worldZ)
    {
        if (!IsWaveActive())
        {
            return transform.position.y;

        }

        float t = Time.time;
        float height = 0f;

        float localX = worldX - transform.position.x;
        float localZ = worldZ - transform.position.z;

        foreach (var wave in waves)
        {
            Vector3 d = CalculateGerstner(wave, localX, localZ, t);
            height += d.y;
        }

        return transform.position.y + height;
    }

    void OnDestroy()
    {
        if (waveToggle != null)
        {
            waveToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

}
