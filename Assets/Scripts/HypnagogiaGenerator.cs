using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HypnagogiaGenerator : MonoBehaviour
{
    public Vector2 size;
    public float scale = 20f;
    [SerializeField] RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        image.texture = GenerateHypnagogia();
    }

    Texture2D GenerateHypnagogia()
    {
        Texture2D texture = new Texture2D((int)size.x, (int)size.y);
        
        TextureImporter textureImporter = new TextureImporter();
        

        texture.alphaIsTransparency = true;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = x / size.x * scale;
        float yCoord = y / size.y * scale;

        float sample = Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord) - 0.5f, 0f, 1f);

        return new Color(sample, sample, sample);
    }
}