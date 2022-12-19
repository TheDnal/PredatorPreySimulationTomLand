using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliderReader : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public void UpdateText()
    {
        float value = this.GetComponent<UnityEngine.UI.Slider>().value;
        value *=10;
        value = Mathf.Round(value);
        value /= 10;
        text.text = value.ToString();   
    }
}
