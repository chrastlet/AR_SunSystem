using UnityEngine;

public class SunSystemScaler : MonoBehaviour
{
    public GameObject sunSystem;
    private float scaleStep = 1.001f;

    public void ScaleUp()
    {
        ApplyScale(scaleStep);
    }

    public void ScaleDown()
    {
        ApplyScale(1 / scaleStep);
    }

    private void ApplyScale(float factor)
    {
        if (sunSystem == null)
        {
            sunSystem = GameObject.FindWithTag("SunSystem");
            if (sunSystem == null) return;
        }

        Vector3 oldScale = sunSystem.transform.localScale;
        Vector3 newScale = oldScale * factor;
        sunSystem.transform.localScale = newScale;

        // Compensate children
        foreach (Transform child in sunSystem.transform)
        {
            child.localPosition = child.localPosition * oldScale.x / newScale.x;
        }
    }
}

