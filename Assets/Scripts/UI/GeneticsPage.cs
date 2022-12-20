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
    public override void InitialisePage()
    {
        gameObject.SetActive(true);
        if(Agent.selectedAgent == null){ResetPage(); return;}
        genome = Agent.selectedAgent.GetGenome();
    }
    public override void UpdatePage()
    {
        if(Agent.selectedAgent == null){ResetPage(); return;}
        genome = Agent.selectedAgent.GetGenome();
        RefreshPage();
    }
    public void ResetPage()
    {
        RefreshGeneUI(rK_Value, 0, 10);
        RefreshGeneUI(speed, 0, 2);
        RefreshGeneUI(size, 0,2);
        RefreshGeneUI(respirationRate, 0,2);
        RefreshGeneUI(visionRadius, 0,6);
        RefreshGeneUI(visionAngle, 0);
        RefreshGeneUI(smellRadius, 0,6);
        RefreshGeneUI(hearingRadius, 0,6);
    }
    public void RefreshPage()
    {
        RefreshGeneUI(rK_Value, genome.rK_Value,10);
        RefreshGeneUI(speed, genome.speed,2);
        RefreshGeneUI(size, genome.size,2);
        RefreshGeneUI(respirationRate, genome.respirationRate,2);
        RefreshGeneUI(visionRadius, genome.visionRadius,6);
        RefreshGeneUI(visionAngle, genome.visionAngle);
        RefreshGeneUI(smellRadius, genome.smellRadius,6);
        RefreshGeneUI(hearingRadius, genome.hearingRadius,6);
    }
    public void RefreshGeneUI(GameObject gene, float value, float max = 1)
    {
        float fill = value /max;
        GameObject slider = gene.transform.GetChild(0).gameObject;
        Image sliderImage = slider.GetComponent<Image>();
        GameObject number = gene.transform.GetChild(2).gameObject;
        number.GetComponent<TextMeshProUGUI>().text = "" +value;
        Vector3 pos;
        //move UI elements into position
        if(fill < 0)
        {
            pos = slider.GetComponent<RectTransform>().anchoredPosition;
            pos.x = negativeXVal;
            slider.GetComponent<RectTransform>().anchoredPosition = pos;
            sliderImage.fillOrigin = 1;
            sliderImage.fillAmount = Mathf.Abs(fill);
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
            sliderImage.fillAmount = Mathf.Abs(fill);
            pos = number.GetComponent<RectTransform>().anchoredPosition;
            pos.x = textPosVal;
            number.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}
