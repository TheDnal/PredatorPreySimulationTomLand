using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GeneticsPage : InspectorPage
{
    private Genome genome;
    private float positiveXVal = 35, negativeXVal = -35;
    private float textPosVal = 75, textNegVal = -75;
    public GameObject rK_Value, speed, size, respirationRate, visionRadius,visionAngle,smellRadius, hearingRadius;
    public Color positiveGeneVal,negativeGeneVal;
    public override void InitialisePage(EntityInspector _Inspector)
    {
        base.InitialisePage(_Inspector);
        genome = Agent.selectedAgent.GetGenome();
        RefreshPage();
    }
    public void RefreshPage()
    {
        RefreshGeneUI(rK_Value, genome.rK_Value);
        RefreshGeneUI(speed, genome.speed);
        RefreshGeneUI(size, genome.size);
        RefreshGeneUI(respirationRate, genome.respirationRate);
        RefreshGeneUI(visionRadius, genome.visionRadius);
        RefreshGeneUI(visionAngle, genome.visionAngle);
        RefreshGeneUI(smellRadius, genome.smellRadius);
        RefreshGeneUI(hearingRadius, genome.hearingRadius);
    }
    public void RefreshGeneUI(GameObject gene, float value)
    {
        GameObject slider = gene.transform.GetChild(0).gameObject;
        Image sliderImage = slider.GetComponent<Image>();
        GameObject number = gene.transform.GetChild(2).gameObject;
        number.GetComponent<TextMeshProUGUI>().text = "" +value;
        Vector3 pos;
        //move UI elements into position
        if(value < 0)
        {
            pos = slider.GetComponent<RectTransform>().anchoredPosition;
            pos.x = negativeXVal;
            slider.GetComponent<RectTransform>().anchoredPosition = pos;
            sliderImage.fillOrigin = 1;
            sliderImage.fillAmount = Mathf.Abs(value);
            pos = number.GetComponent<RectTransform>().anchoredPosition;
            pos.x = textNegVal;
            number.GetComponent<RectTransform>().anchoredPosition = pos;
        }
        else
        {
            pos = slider.GetComponent<RectTransform>().anchoredPosition;
            pos.x = positiveXVal;
            slider.GetComponent<RectTransform>().anchoredPosition = pos;
            sliderImage.fillOrigin = 0;
            sliderImage.fillAmount = Mathf.Abs(value);
            pos = number.GetComponent<RectTransform>().anchoredPosition;
            pos.x = textPosVal;
            number.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}
