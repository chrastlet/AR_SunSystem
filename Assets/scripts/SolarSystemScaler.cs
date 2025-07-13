using UnityEngine;

[ExecuteAlways]
public class SolarSystemScaler : MonoBehaviour
{
    [Range(0.0001f, 1f)]
    public float distanceFactor = 0.001f;

    public Transform[] planets;
    private Vector3[] originalPositions;

    private void OnEnable()
    {
        InitOriginals();
    }

    private void InitOriginals()
    {
        if (planets == null || planets.Length == 0) return;

        originalPositions = new Vector3[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != null)
                originalPositions[i] = planets[i].position * distanceFactor;
        }
    }

    void Update()
    {
        if (originalPositions == null || originalPositions.Length != planets.Length)
            InitOriginals();

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != null)
            {
                planets[i].position = new Vector3(
                    originalPositions[i].x * distanceFactor,
                    originalPositions[i].y * distanceFactor,
                    originalPositions[i].z * distanceFactor
                );
            }
        }
    }
}
