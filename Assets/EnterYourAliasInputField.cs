using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnterYourAliasInputField : InputField
{
    protected override void Awake() {
        base.Awake();
        onValueChanged.AddListener(onChangeAlias);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        placeholder.GetComponent<Text>().text = "";
        caretWidth = 2;
        caretPosition = 10;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if(text == "") {
            placeholder.GetComponent<Text>().text = "Enter your alias...";
        }
    }

    public void onChangeAlias(string alias) {
        if(alias.Length > 0) {
            NewLobbyManager.inst.JoinButton.SetActive(true);
        }
        else {
            NewLobbyManager.inst.JoinButton.SetActive(false);
        }
    }
}
