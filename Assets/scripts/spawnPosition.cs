using UnityEngine;

public class spawnPosition : MonoBehaviour
{
    [Tooltip("Center Eye Anchor der VR-Kamera (optional)")]
    public Transform centerEye;

    void Start()
    {
        if (centerEye == null)
        {
            var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
            if (ovrCameraRig != null)
            {
                centerEye = ovrCameraRig.centerEyeAnchor;
            }
        }

        if (centerEye != null)
        {
            Vector3 forward = centerEye.forward.normalized;

            Vector3 spawnPos = centerEye.position + forward * 2.0f;

            transform.position = spawnPos;

            transform.LookAt(centerEye);
        }
    
    }
}
