using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapMakerScript : MonoBehaviour
{
    public int mapWidth,mapHeight;
    public int seed;
    public float offsetX,offsetY;
    public Slider s_mapWidth, s_mapHeight, s_seed,s_offsetX,s_offsetY;
    public void Start()
    {
        UpdateMap();
    }
    public void BeginSimulation()
    {
        MapGenerator.instance.BeginSimulation();
        this.gameObject.SetActive(false);
    }
    public void UpdateMap()
    {
        UpdateValues();
        Vector2Int mapDimensions = new Vector2Int(mapWidth,mapHeight);
        Vector2 offset = new Vector2(offsetX, offsetY);
        MapGenerator.instance.Generate(mapDimensions, seed, offset);
    }
    public void UpdateValues()
    {
        mapWidth = Mathf.RoundToInt(s_mapWidth.value);
        mapHeight = Mathf.RoundToInt(s_mapHeight.value);
        seed = Mathf.RoundToInt(s_seed.value);
        offsetX = s_offsetX.value;
        offsetY = s_offsetY.value;
    }
}
