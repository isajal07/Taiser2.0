using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdownHandler : MonoBehaviour
{

    public Dropdown dropdown;

    public Difficulty gameDifficulty;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChanged(int index)
    {
        if(shouldTrigger) {
            switch(dropdown.options[index].text.Trim()) {
                case "Novice":
                    gameDifficulty = Difficulty.Novice;
                    break;
                case "Intermediate":
                    gameDifficulty = Difficulty.Intermediate;
                    break;
                case "Advanced":
                    gameDifficulty = Difficulty.Advanced;
                    break;
                default:
                    gameDifficulty = Difficulty.Advanced;
                    break;
            }
            NewLobbyManager.gameDifficulty = gameDifficulty;
        }

    }

    public bool shouldTrigger = true;
    public void SetValueWithoutTrigger(int val)
    {
        shouldTrigger = false;
        dropdown.value = val;
        dropdown.RefreshShownValue();
        shouldTrigger = true;
    }



}
