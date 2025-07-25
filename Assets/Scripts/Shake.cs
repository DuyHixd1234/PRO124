using UnityEngine;

public class Shake : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float frequency = 25f;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * amplitude;
        float y = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * amplitude;

        transform.localPosition = initialPos + new Vector3(x, y, 0f);
    }
}
