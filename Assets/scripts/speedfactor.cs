using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedfactor : MonoBehaviour
{

    private float speedFactor = 1.0f;

    public void setSpeedFactor(float speedfactor)
    {
        this.speedFactor = speedfactor;
    }
    public float getSpeedFactor()
    {
        return speedFactor;
    }
}
