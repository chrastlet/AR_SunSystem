using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.UI;
using Meta.XR.MRUtilityKit;


public class velocityHandler : MonoBehaviour
{
    public UnityEngine.UI.Button prev, next, pause;
    public TMPro.TextMeshProUGUI speed;
    public float speedFactor = 1f;
    private speedfactor sunsystemSpeed;
    private float minSpeedFactor = 0f;
    private float maxSpeedFactor = 10f;

    // Start is called before the first frame update
void Start()
{
    // Buttons verbinden etc.
    prev.onClick.AddListener(OnPrevClicked);
    next.onClick.AddListener(OnNextClicked);
    pause.onClick.AddListener(OnPauseClicked);

    // Starte Suche nach SunSystem
    StartCoroutine(FindSunSystem());
}

    private IEnumerator FindSunSystem()
    {
        while (sunsystemSpeed == null)
        {
            GameObject system = GameObject.FindWithTag("SunSystem");
            if (system != null)
            {
                sunsystemSpeed = system.GetComponent<speedfactor>();
                if (sunsystemSpeed != null)
                {
                    Debug.Log("✅ SunSystem gefunden und Speedfactor-Komponente gespeichert!");

                    // Jetzt erst Buttons aktivieren!
                    prev.onClick.AddListener(OnPrevClicked);
                    next.onClick.AddListener(OnNextClicked);
                    pause.onClick.AddListener(OnPauseClicked);

                    // Optional: UI initialisieren
                    UpdateSpeedUI();

                    yield break;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }    
    private void OnPrevClicked()
    {
        if (speedFactor > minSpeedFactor)
        {
            speedFactor -= 0.5f;
            if (speedFactor < minSpeedFactor)
            {
                speedFactor = minSpeedFactor;
            }
        }

        UpdateSpeedUI();
    }

    private void OnNextClicked()
    {
        if (speedFactor < maxSpeedFactor)
        {
            speedFactor += 0.5f;
            if (speedFactor > maxSpeedFactor)
            {
                speedFactor = maxSpeedFactor;
            }
        }

        UpdateSpeedUI();
    }

    private void OnPauseClicked()
    {
        speedFactor = 0f;
        UpdateSpeedUI();
    }

    private void UpdateSpeedUI()
    {
        // Button-Zustände anpassen
        prev.interactable = speedFactor > minSpeedFactor;
        next.interactable = speedFactor < maxSpeedFactor;

        // Text updaten
        speed.text = speedFactor + "x";

        // An dein System weitergeben
        sunsystemSpeed.setSpeedFactor(speedFactor);
    }


}
