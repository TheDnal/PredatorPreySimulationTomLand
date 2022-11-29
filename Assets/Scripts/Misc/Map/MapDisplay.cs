using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public MapGenerator mapGen;
    public Renderer textureRender;
    void Start()
    {
        textureRender.gameObject.SetActive(false);
    }
    public void DrawNoiseMap(float[,] noiseMap)
    {
        Gradient WaterColorGradient = mapGen.WaterColorGradient;
        Gradient LandcolorGradient = mapGen.LandColorGradient;
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color col;
                if(noiseMap[x,y] < mapGen.WorldSeaLevel)
                {
                    col = WaterColorGradient.Evaluate(noiseMap[x,y]);
                }
                else
                {
                    col = LandcolorGradient.Evaluate(noiseMap[x,y] - mapGen.WorldSeaLevel);
                }
                colourMap[y * width + x] = col;
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
    }
}
