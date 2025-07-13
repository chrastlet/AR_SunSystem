using System;
using UnityEngine;


public class Billboard : MonoBehaviour
{
    [Tooltip("Das Transform, das dieses Billboard immer anschauen soll (z.B. die XR Kamera)")]
    public Transform target;

void LateUpdate()
{
    if (target == null)
    {
        OVRCameraRig ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        if (ovrCameraRig != null)
        {
            target = ovrCameraRig.centerEyeAnchor.transform;
        }
    }

    if (target == null) return;

    Vector3 direction = target.position - transform.position;
    transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
}}