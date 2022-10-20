using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public RectTransform panel;
    private void Awake()
    {
        panel = GetComponent<RectTransform>();
        holdingPosition = panel.anchoredPosition;

    }
    void Start()
    {
    }

    void Update()
    {

    }

    public Vector2 holdingPosition;
    public Vector2 visiblePosition = Vector2.zero;

    public bool _isVisible = false;
    public bool isVisible
    {
        get { return _isVisible; }
        set {
            _isVisible = value;
            if(_isVisible)
                panel.anchoredPosition = visiblePosition;
            else
                panel.anchoredPosition = holdingPosition;
        }
    }

}
