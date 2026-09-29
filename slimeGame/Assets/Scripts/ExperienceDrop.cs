using System.Collections.Generic;
using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    private Dictionary<int, float> xpPair;
    [SerializeField]
    private float xpGain = 0f;
    [SerializeField]
    private List<SpriteRenderer> images;

    void Start()
    {
        float random = Random.value;
        if(random > 0.95f)
        {
            xpGain = 250;
            SetColor(Color.red);
        }
        else if(random > 0.8f)
        {
            xpGain = 75;
            SetColor(Color.yellow);
        }
        else if (random > 0.5f)
        {
            xpGain = 30;
            SetColor(Color.blue);
        }
        else
        {
            xpGain = 10;
            SetColor(Color.grey);
        }

    }


    private void SetColor(Color color)
    {
        foreach (var image in images)
        {
            image.color = color;
        }
    }

    public float GetXP()
    {
        return xpGain;
    }
}
