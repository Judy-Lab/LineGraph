using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;

public class Window_Graph : MonoBehaviour
{
    public bool baseLineIsCenter;
    private float baseYMaximum;
    private float baseGraphHeight;

    [Header("부모 오브젝트")]
    public RectTransform graphContainer;
    public RectTransform graphWidthLine;
    public RectTransform lineParent;
    public RectTransform averageLine;
    public RectTransform deviationArea;

    [Header("UI_Extension")]
    public UILineRenderer lineRenderer;

    [Header("X축 옵션")]
    public RectTransform labelTemplateX;
    public float fontSizeX = 25f;
    public float paddingMultiplyX = 0.8f;
    public float widthMultiplyX = 0.55f;
    public bool startNumber2 = false;

    private float startNumber;

    [Header("Y축 옵션")]
    public RectTransform labelTemplateY;
    public float fontSizeY = 25f;
    public float paddingMultiplyY = 1.4f;
    public float heightMultiplyY = 0.6f;
    public int separatorCount = 10;
    public string stringFormatY = "F1";

    private float graphHeight;
    private float yMaximum;
    private float xSize;

    private List<RectTransform> xLabelList;
    private List<RectTransform> yLabelList;

    private void Setting()
    {
        if (xLabelList == null)
        {
            xLabelList = new List<RectTransform>();
        }
        if (yLabelList == null)
        {
            yLabelList = new List<RectTransform>();
        }

        if (baseLineIsCenter)
        {
            baseYMaximum = 2;
            baseGraphHeight = 0.5f;

            graphWidthLine.anchoredPosition = new Vector2(0, graphContainer.sizeDelta.y * 0.5f);
        }
        else
        {
            baseYMaximum = 1;
            baseGraphHeight = 0;

            graphWidthLine.anchoredPosition = new Vector2(0, 0);
        }

        if (startNumber2)
        {
            startNumber = 1;
        }
        else
        {
            startNumber = 0;
        }
    }

    public void ShowGraph(List<float> _list, float _average, float _deviation)
    {
        Setting();

        if (!GraphEmptyCheck(_list))
        {
            return;
        }

        graphHeight = graphContainer.sizeDelta.y;
        yMaximum = AbsMax(_list);
        xSize = (graphContainer.sizeDelta.x) / (_list.Count + startNumber);

        SetYLabel();
        SetDirty(SetPoints(_list));

        SetAverage(_average);
        SetDeviation(_average, _deviation);
    }

    private Vector2[] SetPoints(List<float> _list)
    {
        Vector2[] points = new Vector2[_list.Count];
        for (int i = 0; i < _list.Count; i++)
        {
            float xPosition = (i + startNumber) * xSize;
            float yPosition = 0;

            if (_list[i] != 0)
            {
                yPosition = (_list[i] / (yMaximum * baseYMaximum)) * graphHeight;
            }

            points[i] = new Vector2(xPosition, yPosition + (graphHeight * baseGraphHeight));

            // labelX
            RectTransform label;
            if (xLabelList.Count <= i)
            {
                label = Instantiate(labelTemplateX);
                label.SetParent(graphContainer);

                xLabelList.Add(label);
            }
            else
            {
                label = xLabelList[i];
            }

            TextMeshProUGUI tmpX = label.GetComponent<TextMeshProUGUI>();
            tmpX.text = (i + 1).ToString();
            tmpX.fontSize = fontSizeX;

            label.anchoredPosition = new Vector2(xPosition - (label.sizeDelta.x * widthMultiplyX)
                , -label.sizeDelta.y * paddingMultiplyX);
        }

        if (xLabelList.Count > _list.Count)
        {
            for (int i = _list.Count; i < xLabelList.Count; i++)
            {
                Destroy(xLabelList[i].gameObject);
            }
        }
        return points;
    }

    private void SetYLabel()
    {
        int separete = separatorCount - 1;
        for (int i = 0; i <= separete; i++)
        {
            // labelY
            RectTransform label;
            if (yLabelList.Count <= i)
            {
                label = Instantiate(labelTemplateY);
                label.SetParent(graphContainer);

                yLabelList.Add(label);
            }
            else
            {
                label = yLabelList[i];
            }

            float normalizedValue = (i - (separete * baseGraphHeight)) / separete;
            label.anchoredPosition = new Vector2(-label.sizeDelta.x * paddingMultiplyY
                , (normalizedValue * graphHeight) + (graphHeight * baseGraphHeight) - (label.sizeDelta.y * heightMultiplyY));

            TextMeshProUGUI tmpY = label.GetComponent<TextMeshProUGUI>();
            tmpY.text = (normalizedValue * yMaximum).ToString(stringFormatY);
            tmpY.fontSize = fontSizeY;
        }

        if (yLabelList.Count > separatorCount)
        {
            for (int i = separete; i < yLabelList.Count; i++)
            {
                Destroy(yLabelList[i].gameObject);
            }
        }
    }

    private void SetDirty(Vector2[] _points)
    {
        lineRenderer.Points = _points;
        lineRenderer.SetAllDirty();
    }

    private float AbsMax(List<float> _list)
    {
        float max = _list.Max();
        float min = Mathf.Abs(_list.Min());

        return (max > min) ? max : min;
    }

    private void SetAverage(float _average)
    {
        float yPosition = (_average / (yMaximum * baseYMaximum)) * graphHeight;
        averageLine.anchoredPosition = new Vector2(0, yPosition + (graphHeight * baseGraphHeight));
    }

    private void SetDeviation(float _average, float _deviation)
    {
        float yPosition = (_average / (yMaximum * baseYMaximum)) * graphHeight;

        deviationArea.sizeDelta = new Vector2(deviationArea.sizeDelta.x,
            (_deviation / (yMaximum * baseYMaximum)) * graphHeight * 2);
        deviationArea.anchoredPosition = new Vector2(0, yPosition + (graphHeight * baseGraphHeight)
            - (deviationArea.sizeDelta.y * 0.5f));
    }

    private bool GraphEmptyCheck(List<float> _list)
    {
        int count;

        if (_list.Count > 2)
        {
            lineRenderer.gameObject.SetActive(true);
            averageLine.gameObject.SetActive(true);
            deviationArea.gameObject.SetActive(true);

            count = (yLabelList.Count > xLabelList.Count) ? yLabelList.Count : xLabelList.Count;
            for (int i = 0; i < count; i++)
            {
                if (xLabelList.Count > i)
                {
                    xLabelList[i].gameObject.SetActive(true);
                }
                if (yLabelList.Count > i)
                {
                    yLabelList[i].gameObject.SetActive(true);
                }
            }
            return true;
        }

        lineRenderer.gameObject.SetActive(false);
        averageLine.gameObject.SetActive(false);
        deviationArea.gameObject.SetActive(false);

        count = (yLabelList.Count > xLabelList.Count) ? yLabelList.Count : xLabelList.Count;
        for (int i = 0; i < count; i++)
        {
            if (xLabelList.Count > i)
            {
                xLabelList[i].gameObject.SetActive(false);
            }
            if (yLabelList.Count > i)
            {
                yLabelList[i].gameObject.SetActive(false);
            }
        }
        return false;
    }
}
