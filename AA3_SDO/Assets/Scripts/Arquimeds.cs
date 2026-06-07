using UnityEngine;

public class Arquimeds : MonoBehaviour
{
    [Header("Object's properties")]

    public float mass = 800f; // kg
    public Vector3 objectDimensions = Vector3.one;
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    

    [Header("fluid's properties")]
    public float densityFluid = 1000f; //kg/m^3
    public float waterLevel = 0f;
    private float volumeDisplaced;
    public float dragCoefficient = 10.0f;

    [Header("physics' properties")]
    public float gravity = 9.81f; // m/s^2
    public Vector3 initialPosition = Vector3.zero;
    public Vector3 initialVelocity = Vector3.zero;

    [Header("time's properties")]
    public float stepTime = 0.01f;
    public float totalTime = 1000f;
    private float time = 0f;

    public bool simulationEnabled = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        initialPosition = position;

        velocity = initialVelocity;
        acceleration = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        if (!simulationEnabled) return;

        time += Time.deltaTime;

        while (time >= stepTime)
        {
            calculateSubmergedVolume();
            calculateAcceleration();
            calculatePositionandVelocity();
            time -= stepTime;
        }
        transform.position = new Vector3(initialPosition.x, position.y, initialPosition.z);

    }

    void calculatePositionandVelocity() {

        velocity += acceleration * stepTime;
        //position += velocity*stepTime + 0.5f*acceleration*stepTime*stepTime;
        position += velocity * stepTime;// + 0.5f*acceleration*stepTime*stepTime;

    }

    void calculateAcceleration()
    {

        Vector3 gravForce = new Vector3(0, -mass * gravity, 0);
        Vector3 bouyantForce = new Vector3(0, densityFluid * volumeDisplaced * gravity, 0);

        Vector3 netForce = gravForce + bouyantForce;

        //Debug.Log($"Grav: {gravForce.y:F2} | Empuje: {bouyantForce.y:F2} | Drag: {dragForce.y:F2} | Vol: {volumeDisplaced:F4}");

        acceleration = netForce / mass;

        // Aplica el drag directamente sobre la velocidad, no como fuerza
        velocity *= Mathf.Clamp01(1f - dragCoefficient * stepTime);

    }

    void calculateSubmergedVolume() { 
        
        float objectBottomY = position.y - 0.5f * objectDimensions.y;
        float objectTopY = position.y + 0.5f * objectDimensions.y;

        if (objectBottomY >= waterLevel)
        {

            volumeDisplaced = 0;
        }
        else if (objectTopY <= waterLevel)
        {

            volumeDisplaced = objectDimensions.x * objectDimensions.y * objectDimensions.z;
        }
        else { 
        
            float submergedHeight = waterLevel - objectBottomY;

            volumeDisplaced =  objectDimensions.x * submergedHeight * objectDimensions.z;
        }
   
    }
    public void SetWaterLevel(float newWaterLevel)
    {
        waterLevel = newWaterLevel;
    }
    void OnDrawGizmos()
    {
        DrawCube(transform.position, objectDimensions);   
    }

    void DrawCube(Vector3 center, Vector3 dimensions)
    {
        Vector3[] corners = new Vector3[8] {

           center + new Vector3(-0.5f*dimensions.x, -0.5f*dimensions.y, -0.5f*dimensions.z ),
           center + new Vector3(0.5f*dimensions.x, -0.5f*dimensions.y, -0.5f*dimensions.z ),
           center + new Vector3(0.5f*dimensions.x, 0.5f*dimensions.y, -0.5f*dimensions.z ),
           center + new Vector3(-0.5f*dimensions.x, 0.5f*dimensions.y, -0.5f*dimensions.z ),
           center + new Vector3(-0.5f*dimensions.x, -0.5f*dimensions.y, 0.5f*dimensions.z ),
           center + new Vector3(0.5f*dimensions.x, -0.5f*dimensions.y, 0.5f*dimensions.z ),
           center + new Vector3(0.5f*dimensions.x, 0.5f*dimensions.y, 0.5f*dimensions.z ),
           center + new Vector3(-0.5f*dimensions.x, 0.5f*dimensions.y, 0.5f*dimensions.z ),
        };

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

        Gizmos.DrawLine(corners[4], corners[5]);
        Gizmos.DrawLine(corners[5], corners[6]);
        Gizmos.DrawLine(corners[6], corners[7]);
        Gizmos.DrawLine(corners[7], corners[4]);


        Gizmos.DrawLine(corners[4], corners[0]);
        Gizmos.DrawLine(corners[5], corners[1]);
        Gizmos.DrawLine(corners[6], corners[2]);
        Gizmos.DrawLine(corners[7], corners[3]);
    }


}
