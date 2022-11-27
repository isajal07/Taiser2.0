using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class FileSystems : MonoBehaviour
{

    public static FileSystems inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FileWriter;

    public void WriteFileToServer(string filename, string content)
    {
        FileWriter = WriteToServer(filename, content);
        StartCoroutine(FileWriter);
    }

    IEnumerator WriteToServer(string filename, string content)
    {
        XmlDocument map = new XmlDocument();
        map.LoadXml("<level></level>");
        byte[] data = Encoding.UTF8.GetBytes(content);
        Debug.Log("Writing FileName: " + filename);

        WWWForm form = new WWWForm();
        Debug.Log("Created new WWW Form");
        form.AddField("action", "level upload");
        form.AddField("file", "file");
        form.AddBinaryData("file", data, filename, "text/csv");
        Debug.Log("Data added");
        WWW w = new WWW("https://www.cse.unr.edu/~sajal/T2/Dataload.php", form);
        yield return w;

        if(w.error != null) {
            Debug.Log("Error: " + w.error);
            Debug.Log(w.text);
        } else {
            Debug.Log("No errors");
            //Debug.Log(w.text);
            if(w.uploadProgress == 1 || w.isDone) {
                yield return new WaitForSeconds(5);
                Debug.Log("Waited five seconds");
            }
        }
    }

    public string FileContent;
    IEnumerator FileReader;
    public string ReadFileFromServer(string filename)
    {
        FileReader = ReadFromServer(filename);
        StartCoroutine(FileReader);
        return null;
    }

    IEnumerator ReadFromServer(string filename)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://www.cse.unr.edu/~sajal/T2/T2Data/" + filename);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            FileContent = www.downloadHandler.text;
        }
    }
}
