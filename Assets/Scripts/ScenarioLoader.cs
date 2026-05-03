using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioLine
{
    public string characterName = "";
    public string messageText = "";
    public string spriteName = "";
    public string command = "";
    public string param1 = "";
    public string param2 = "";
    public string param3 = "";
}
public class ScenarioLoader : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private string csvFileName = "scenario";

    public List<ScenarioLine> scenarioData = new List<ScenarioLine>();
    public Dictionary<string, int> labelList = new Dictionary<string, int>();

    public void LoadScenarioData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null) return;
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length >= 2)
            {
                ScenarioLine lineData = new ScenarioLine();
                lineData.characterName = parts[0];
                lineData.messageText = parts[1];
                if (parts.Length > 2) lineData.spriteName = parts[2];
                if (parts.Length > 3) lineData.command = parts[3];
                if (parts.Length > 4) lineData.param1 = parts[4];
                if (parts.Length > 5) lineData.param2 = parts[5];
                if (parts.Length > 6) lineData.param3 = parts[6];

                scenarioData.Add(lineData);

                if (lineData.command == "Label")
                {
                    labelList[lineData.param1] = scenarioData.Count - 1;
                }
            }
        }
    }
}


