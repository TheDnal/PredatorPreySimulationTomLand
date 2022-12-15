using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class popScript : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private List<GameObject> points = new List<GameObject>();
    private List<int> dataPoints = new List<int>();
    public RectTransform graphContainer;
    public TextMeshProUGUI populationCounter;
    void Start()
    {
        StartCoroutine(i_dataCollector());
    }
    private IEnumerator i_dataCollector()
    {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            int population = EntitySpawner.instance.currentPopulation;
            dataPoints.Add(population);
            ShowGraph(dataPoints);
            populationCounter.text = "Population : " + population;
        }
    }
    private void CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject= new GameObject("Circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer,false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11,11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        points.Add(gameObject);
    }
    void Update()
    {

    }
    private void ShowGraph(List<int> valueList)
    {
        for(int i =0 ; i < points.Count - 1; i++)
        {
            Destroy(points[i]);
        }
        for(int i = 0; i < valueList.Count; i++)
        {
            float xPos = i;
            float yPos = valueList[i];
            CreateCircle(new Vector2(xPos, yPos));
        }
    }
}
