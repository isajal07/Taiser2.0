using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Utils : MonoBehaviour
{
    public bool generateBoolean()
    {
        return Random.Range(0, 2) != 0;
    }

    public int generateWaitingTime()
    {
        int[] times = new[] { 1000,  1500, 2000, 2500, 3000 };
        int indexOfTime = Random.Range(0, 4);
        return times[indexOfTime];
    }

    public string generatePlayerName()
    {
        List<string> HumanNames = new List<string> { "Alex", "Drew", "Kennedy", "Jordan", "Emerson", "Morgan", "Kit", "Sol", "Revel", "Angel", "Riley", "Peyton", "Taylor", "Casey", "Charlie", "Blake" };
        int choice = Random.Range(0, HumanNames.Count);
        return HumanNames[choice];
    }
}