using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    public Window_Graph[] graphs;

    private void Awake()
    {
        List<float>[] list = new List<float>[4];

        for (int i = 0; i < 4; i++)
        {
            list[i] = new List<float>();
            float rangeMin = graphs[i].baseLineIsCenter ? -6.0f : 0;
            for (int j = 0; j < 10; j++)
            {
                list[i].Add(Random.Range(rangeMin, 6.0f));
            }

            float average = list[i].Average();
            graphs[i].ShowGraph(list[i], average, CalculateDeviation(list[i], average));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            List<float>[] list = new List<float>[4];

            for (int i = 0; i < 4; i++)
            {
                list[i] = new List<float>();
                float rangeMin = graphs[i].baseLineIsCenter ? -6.0f : 0;
                for (int j = 0; j < 10; j++)
                {
                    list[i].Add(Random.Range(rangeMin, 6.0f));
                }

                float average = list[i].Average();
                graphs[i].ShowGraph(list[i], average, CalculateDeviation(list[i], average));
            }
        }
    }

    private float CalculateDeviation(List<float> _list, float _average)
    {
        float average = _average;
        float sumOfDerivation = 0;
        foreach (float value in _list)
        {
            sumOfDerivation += (value) * (value);
        }
        float sumOfDerivationAverage = sumOfDerivation / _list.Count;

        return Mathf.Sqrt(sumOfDerivationAverage - (average * average));
    }
}
