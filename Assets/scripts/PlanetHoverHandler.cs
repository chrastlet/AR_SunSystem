using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(RayInteractable))]
public class PlanetHoverHandler : MonoBehaviour
{
    [Tooltip("GameObject that will appear/disappear on hover")]
    public GameObject HoverCylinder;

    private InteractableUnityEventWrapper _events;

    void Awake()
    {

        _events = GetComponent<InteractableUnityEventWrapper>();

        if (_events == null)
        {
            Debug.LogError($"[PlanetHoverHandler] ❌ InteractableUnityEventWrapper NOT FOUND on '{gameObject.name}'! Please add it in the Inspector!");
        }
        else
        {
            Debug.Log($"[PlanetHoverHandler] ✅ InteractableUnityEventWrapper FOUND on '{gameObject.name}'.");
        }
    }

    void Start()
    {
        Debug.Log($"[PlanetHoverHandler] 🟢 Start called on '{gameObject.name}'.");

        if (HoverCylinder != null)
        {
            HoverCylinder.SetActive(false);
            Debug.Log($"[PlanetHoverHandler] ✅ HoverCylinder '{HoverCylinder.name}' initialized as INACTIVE.");
        }
        else
        {
            Debug.LogWarning($"[PlanetHoverHandler] ⚠️ HoverCylinder reference is NULL on '{gameObject.name}'! Please assign it in the Inspector!");
        }

        if (_events != null)
        {
            Debug.Log($"[PlanetHoverHandler] ✅ Registering hover event listeners for '{gameObject.name}'.");
            _events.WhenHover.AddListener(OnHoverStart);
            _events.WhenUnhover.AddListener(OnHoverEnd);
        }
        else
        {
            Debug.LogError($"[PlanetHoverHandler] ❌ Cannot register hover events because InteractableUnityEventWrapper is missing on '{gameObject.name}'!");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[PlanetHoverHandler] ⚫ OnDestroy called on '{gameObject.name}'.");

        if (_events != null)
        {
            _events.WhenHover.RemoveListener(OnHoverStart);
            _events.WhenUnhover.RemoveListener(OnHoverEnd);
            Debug.Log($"[PlanetHoverHandler] ✅ Hover event listeners REMOVED from '{gameObject.name}'.");
        }
        else
        {
            Debug.LogWarning($"[PlanetHoverHandler] ⚠️ _events was NULL in OnDestroy on '{gameObject.name}'. Nothing to remove.");
        }
    }

    public void OnHoverStart()
    {
        Debug.Log($"[PlanetHoverHandler] ▶️ OnHoverStart triggered on '{gameObject.name}'.");

        if (HoverCylinder != null)
        {
            HoverCylinder.SetActive(true);
            Debug.Log($"[PlanetHoverHandler] ✅ HoverCylinder '{HoverCylinder.name}' is now ACTIVE (visible) on '{gameObject.name}'.");
        }
        else
        {
            Debug.LogWarning($"[PlanetHoverHandler] ⚠️ HoverCylinder is NULL in OnHoverStart on '{gameObject.name}'!");
        }
    }

    public void OnHoverEnd()
    {
        Debug.Log($"[PlanetHoverHandler] ▶️ OnHoverEnd triggered on '{gameObject.name}'.");

        if (HoverCylinder != null)
        {
            HoverCylinder.SetActive(false);
            Debug.Log($"[PlanetHoverHandler] ✅ HoverCylinder '{HoverCylinder.name}' is now INACTIVE (hidden) on '{gameObject.name}'.");
        }
        else
        {
            Debug.LogWarning($"[PlanetHoverHandler] ⚠️ HoverCylinder is NULL in OnHoverEnd on '{gameObject.name}'!");
        }
    }
}
