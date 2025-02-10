using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parameter : MonoBehaviour
{
    public static Parameter Instance { get; private set;}
    public Slider PUSlider, ESSlider, MSSlider, QLSlider, SUSlider;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetPUValue(int value)
    {
        PUSlider.value += value;
    }
    public void SetESValue(int value)
    {
        ESSlider.value += value;
    }
    public void SetMSValue(int value)
    {
        MSSlider.value += value;
    }
    public void SetQLValue(int value)
    {
        QLSlider.value += value;
    }
    public void SetSUValue()
    {
        float SUS = (ESSlider.value + MSSlider.value + QLSlider.value) / 3;
        SUSlider.value = SUS;
    }
}
