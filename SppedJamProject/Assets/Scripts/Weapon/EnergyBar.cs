using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetEnergy(float value)
    {
        value = Mathf.Clamp01(value);
        slider.value = value;
    }
}
