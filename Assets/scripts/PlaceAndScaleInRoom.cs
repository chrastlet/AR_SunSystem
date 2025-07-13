using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInPlayArea : MonoBehaviour
{
    private OVRBoundary boundary;

    void Start()
    {
        boundary = new OVRBoundary();

        Vector3 playAreaSize = boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        Debug.Log($"✅ PlayArea Size: {playAreaSize}");

        for (int i = 0; i < 5; i++)
        {
            SpawnCube(playAreaSize);
        }
    }

    void SpawnCube(Vector3 playAreaSize)
    {
        float halfWidth = playAreaSize.x / 2f;
        float halfDepth = playAreaSize.z / 2f;

        Vector3 randomPos = new Vector3(
            Random.Range(-halfWidth, halfWidth),
            0.2f,
            Random.Range(-halfDepth, halfDepth)
        );

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = randomPos;
        cube.transform.localScale = Vector3.one * 0.2f;
        cube.GetComponent<Renderer>().material.color = Color.green;

        Debug.Log($"✅ Spawned cube at {randomPos}");
    }
}
