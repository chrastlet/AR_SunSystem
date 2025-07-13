using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [Header("Rotation um eigene Achse")]
    public float rotationSpeed;
    [Range(0, 1)]
    public float[] rotationAxis;

    [Header("Berechnete Orbitgeschwindigkeit")]
    public float orbitSpeed;

    private float radius = 0.0f;
    private float currentAngle = 0f;
    private float curVelocityFactor = 1f;
    private speedfactor factorscript;

    public GameObject centerObject;

    void Start()
    {
        // Abstand zur Sonne direkt aus Positionen berechnen
        radius = Vector2.Distance(
            new Vector2(transform.localPosition.x, transform.localPosition.z),
            new Vector2(centerObject.transform.localPosition.x, centerObject.transform.localPosition.z)
        );

        factorscript = transform.parent.GetComponent<speedfactor>();

        ComputeOrbitSpeed();
    }

    void ComputeOrbitSpeed()
    {
        // Hole globalen Faktor (z. B. Zeitraffer)
        float globalFactor = 1f;
        if (factorscript != null) globalFactor = factorscript.getSpeedFactor();

        // Vermeide Division durch 0
        if (radius <= 0.0001f)
        {
            orbitSpeed = 0f;
            Debug.LogWarning($"{gameObject.name}: Radius ist zu klein – orbitSpeed auf 0 gesetzt.");
            return;
        }

        // Kepler'sches Gesetz: Umlaufzeit ~ Abstand^(3/2)
        // orbitSpeed ~ 1 / Abstand^(3/2)
        float speedFactor = 1f / Mathf.Pow(radius, 1.5f);

        orbitSpeed = speedFactor * globalFactor;

        Debug.Log($"{gameObject.name}: radius = {radius}, orbitSpeed berechnet = {orbitSpeed}");
    }

    void Update()
    {
        if (factorscript != null) curVelocityFactor = factorscript.getSpeedFactor();

        // Rotation um eigene Achse
        transform.Rotate(new Vector3(rotationAxis[0], rotationAxis[1], rotationAxis[2]),
                         (rotationSpeed * curVelocityFactor) * Time.deltaTime);

        // Umlaufbewegung
        currentAngle += (orbitSpeed * curVelocityFactor) * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        float rad = currentAngle * Mathf.Deg2Rad;

        float x = centerObject.transform.localPosition.x + radius * Mathf.Cos(rad);
        float z = centerObject.transform.localPosition.z + radius * Mathf.Sin(rad);

        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
    }
}
