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

    public float generateWaitingTime()
    {
        float[] times = new[] { 1.0f,  1.5f, 2.0f, 2.5f, 3.0f };
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