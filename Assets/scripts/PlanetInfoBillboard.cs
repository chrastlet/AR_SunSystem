using System;
using System.Numerics;
using Unity.XR.CoreUtils;
using UnityEngine;


public class PlanetInfoBillboard : MonoBehaviour
{
    private Transform target;

    [Tooltip("Das Planet-Transform, um das sich das Billboard orientiert")]
    public Transform planet;
    private UnityEngine.Vector3 lastPlanetSize;
    private UnityEngine.Vector3 transformStartSize;
    void Start()
    {
        transformStartSize = transform.localScale;
        transform.localScale = transformStartSize.Divide(new UnityEngine.Vector3(planet.transform.localScale.x,planet.transform.localScale.x,planet.transform.localScale.x));
        if (planet != null) lastPlanetSize = planet.localScale;

    }
    void LateUpdate()
    {
        OVRCameraRig ovrCameraRig = FindObjectOfType<OVRCameraRig>();

        if (ovrCameraRig != null)
        {
            target = ovrCameraRig.centerEyeAnchor.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("❌ Billboard target is not set! Please assign a target Transform.");
            return;
        }

        // Billboard richtet sich immer zum Ziel aus
        transform.LookAt(target.position);

        // Optional: Billboard immer aufrecht halten
        UnityEngine.Vector3 euler = transform.eulerAngles;
        //euler.y += 180f;
        transform.eulerAngles = euler;

        // Align on the right side of the planet from player's view, with world-space offset
        if (planet != null)
        {
            if (lastPlanetSize != planet.localScale)
            {
                transform.localScale = transformStartSize.Divide(new UnityEngine.Vector3(planet.transform.localScale.x,planet.transform.localScale.x,planet.transform.localScale.x));
                lastPlanetSize = planet.localScale;

            }
            UnityEngine.Vector3 planetPosition = planet.position;
            UnityEngine.Vector3 CamPos = ovrCameraRig.centerEyeAnchor.transform.position;
            
            // Stable "forward" from planet to camera on XZ plane
            UnityEngine.Vector3 forward = (CamPos - planetPosition);
            forward.y = 0f;
            if (forward == UnityEngine.Vector3.zero) forward = UnityEngine.Vector3.forward;
            forward = forward.normalized;

            // Always "right" from camera's view in XZ plane
            UnityEngine.Vector3 right = -UnityEngine.Vector3.Cross(UnityEngine.Vector3.up, forward).normalized;

            // Compute effective world radius of the planet
            float planetRadiusWorld = planet.lossyScale.x * 0.5f;

            // Compute effective world radius of the billboard itself
            float billboardRadiusWorld = transform.lossyScale.x; ;

            // Calculate total desired offset in world space
            float totalOffset = planetRadiusWorld + billboardRadiusWorld;

            // Set world-space position directly (ignores parent scale)
            transform.position = planetPosition + (right * totalOffset);

}
        else
        {
            Debug.LogWarning("❌ Planet Transform is not set! Please assign a planet Transform.");
        }
    }
}
