using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInOut : MonoBehaviour
{

    public bool horizontal;
    public float valueIn;
    public float valueOut;
    public float speed;
    public bool isOn;
    RectTransform rect;
    private void Start()
    {
        rect = GetComponent<RectTransform>();

    }
    public void MovePanel(bool on)
    {
        isOn = on;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 pos = rect.anchoredPosition;
        if (horizontal)
        {
            pos.x = Mathf.Lerp(pos.x, isOn ? valueIn : valueOut, Time.deltaTime * speed);
        }
        else
        {
            pos.y = Mathf.Lerp(pos.y, isOn ? valueIn : valueOut, Time.deltaTime * speed);
        }
        rect.anchoredPosition = pos;
    }
}
