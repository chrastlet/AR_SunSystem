using UnityEngine;

public class FitToChildrenBounds : MonoBehaviour
{
    void Start()
    {
        Bounds combinedBounds = GetCombinedRendererBounds();

        if (combinedBounds.size == Vector3.zero)
        {
            Debug.LogWarning("No renderers found in children. Nothing to fit.");
            return;
        }

        Vector3 currentSize = combinedBounds.size;

        // Faktor um auf 1x1x1 zu bringen
        Vector3 scaleFactor = new Vector3(
            1f / currentSize.x,
            1f / currentSize.y,
            1f / currentSize.z
        );

        // Optional: Einheitliche Skalierung auf kleinste Achse
        float uniformScale = Mathf.Min(scaleFactor.x, scaleFactor.y, scaleFactor.z);

        transform.localScale = transform.localScale * uniformScale;

        Debug.Log($"New localScale: {transform.localScale}");
    }

    Bounds GetCombinedRendererBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }
}

