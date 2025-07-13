using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(InteractableUnityEventWrapper))]
public class ClickToShowHandler : MonoBehaviour
{
    [Tooltip("This GameObject will be shown when selected/clicked")]
    public GameObject ElementToShow;

    private InteractableUnityEventWrapper _events;

    void Awake()
    {
        _events = GetComponent<InteractableUnityEventWrapper>();
    }

    void Start()
    {
        if (ElementToShow != null)
        {
            ElementToShow.SetActive(false);
        }

        if (_events != null)
        {
            _events.WhenSelect.AddListener(OnClick);
        }
    }

    private void OnDestroy()
    {
        if (_events != null)
        {
            _events.WhenSelect.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (ElementToShow != null)
        {
            if (ElementToShow.activeSelf)
                ElementToShow.SetActive(false);
            else
                ElementToShow.SetActive(true);
        }
    }


}
