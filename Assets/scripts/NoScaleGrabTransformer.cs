using UnityEngine;
using Oculus.Interaction;

public class NoScaleRayGrabTransformer : MonoBehaviour, ITransformer
{
    private IGrabbable _grabbable;
    
    private Pose _grabStartControllerPose;
    private Pose _grabStartObjectPose;
    private Vector3 _localOffset;
    private Vector3 _savedScale;
    [Tooltip("Optional: Canvas or GameObject to hide on grab start")]
    public GameObject canvasToHide;

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
    }

    public void BeginTransform()
    {
        if (_grabbable.GrabPoints.Count > 0)
        {
            
            _grabStartControllerPose = _grabbable.GrabPoints[0];
            _grabStartObjectPose = new Pose(_grabbable.Transform.position, _grabbable.Transform.rotation);

            // Store offset in controller local space
            _localOffset = Quaternion.Inverse(_grabStartControllerPose.rotation) * (_grabStartObjectPose.position - _grabStartControllerPose.position);

            // SAVE scale at start
            _savedScale = _grabbable.Transform.localScale;
            // Hide the canvas when grab starts
            if (canvasToHide != null)
            {
                canvasToHide.SetActive(false);
            }

        }
    }

    public void UpdateTransform()
    {
        if (_grabbable.GrabPoints.Count == 0) return;

        Pose currentControllerPose = _grabbable.GrabPoints[0];
        Vector3 newWorldPos = currentControllerPose.position + currentControllerPose.rotation * _localOffset;

        // APPLY only position and rotation
        _grabbable.Transform.SetPositionAndRotation(newWorldPos, currentControllerPose.rotation);

        // FORCE saved scale every frame
        _grabbable.Transform.localScale = _savedScale;
    }

    public void EndTransform()
    {
        // Optionally, restore scale again at end
        if (_grabbable != null)
        {
            _grabbable.Transform.localScale = _savedScale;
        }
    }
}
