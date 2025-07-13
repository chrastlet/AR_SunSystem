using UnityEngine;
using Oculus.Interaction;

public class PlanetRaySelectHandler : MonoBehaviour
{
    [SerializeField] GameObject infoBillboard;

    private RayInteractable _rayInteractable;

    private void Awake()
    {
        _rayInteractable = GetComponent<RayInteractable>();
    }

    private void OnSelected()
    {
        Debug.Log($"✅ Selected {gameObject.name}");
        infoBillboard.SetActive(true);
        infoBillboard.transform.position = transform.position + Vector3.up * 0.5f;
    }

    private void OnUnselected()
    {
        infoBillboard.SetActive(false);
    }
}
