using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        slider.onValueChanged.AddListener((value) =>
        {
            text.text = value.ToString("F2");
        });
    }
}
