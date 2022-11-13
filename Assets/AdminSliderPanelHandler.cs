using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminSliderPanelHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AdminSlider = transform.gameObject.GetComponentInChildren<Slider>();
        if(SliderValueText == null)
            Debug.Log("Hook up text UI widget to variable SliderValueText");
        if(isInt)
            FormatString = "0";
        else
            FormatString = "0.00";
        SliderValueText.text = AdminSlider.value.ToString(FormatString);
        VariableNameText.text = ParameterName.ToString();
    }

    public Text VariableNameText;
    public Slider AdminSlider;
    public Text SliderValueText;
    string FormatString;
    public ParameterNames ParameterName;

    public bool isInt = false;
    //    public string VariableName;
    public float Value;
    // Update is called once per frame
    void Update()
    {
        SliderValueText.text = AdminSlider.value.ToString(FormatString);
    }

    public void ReadAndSet(string varName, float val)
    {
        Value = val;
        AdminSlider.value = val;
        VariableNameText.text = varName.Trim();
    }

    public void MakeReadyToWrite()
    {
        Value = AdminSlider.value;
    }

}
