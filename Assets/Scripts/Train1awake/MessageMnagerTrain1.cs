
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageManagerTrain1 : MonoBehaviour
{
    // ⭕ 監督が使う「連絡先」はこの2つだけ！
    [SerializeField] private ScenarioLoader scenarioLoader;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private GameObject statusGroup;
    [SerializeField] private DiceManager diceManager;

    private int currentLine = 0;
    private bool isWaitingForChoice = false;
    private bool isWaitingForSearch = false;
    private string currentFullText = "";

    void Start()
    {
        // UIの初期化（全部消す）

        uiManager.SetChoicePanelActive(false);
        uiManager.ResetAllChoices();
        scenarioLoader.LoadScenarioData();
        ShowNextLine();

    }

    void Update()
    {
        if (isWaitingForChoice) return;
        if (isWaitingForSearch) return;

        if (Input.GetMouseButtonDown(0))
        {
            // uiManagerに「今文字打ってる？」と聞く
            if (uiManager.IsTyping)
            {
                // 打っているならスキップを命じる
                uiManager.FinishTyping(currentFullText);
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    private void ShowNextLine()
    {
        if (currentLine >= scenarioLoader.scenarioData.Count)
        {
            uiManager.ClearBubble();     
            darkPanel.SetActive(true);   
            statusGroup.SetActive(true); 

            return;
        }

        ScenarioLine currentData = scenarioLoader.scenarioData[currentLine];
        string command = currentData.command;
        string param1 = currentData.param1;
        string param2 = currentData.param2;
        string param3 = currentData.param3;

        if (command == "Label")
        {
            currentLine++;
            ShowNextLine();
            return;
        }

        if (command == "Search")
        {
            uiManager.ClearBubble();
            isWaitingForSearch = true;
            return;
        }

        if (command == "Jump")
        {
            JumpTo(param1);
            return;
        }

        if (command == "Choice")
        {
            ShowChoices();
            return;
        }

        if (command == "Roll")
        {
            ExecuteRoll(param1, param2, param3);
            return;
        }

        if (command == "ChangeScene")
        {
            SceneManager.LoadScene(param1);
            return;
        }



        // --- 普通のセリフ表示処理 ---
        currentFullText = currentData.messageText;

        // X座標とY座標の計算
        Vector2 pos = Vector2.zero;
        if (float.TryParse(currentData.param1, out float posX) && float.TryParse(currentData.param2, out float posY))
        {
            pos = new Vector2(posX, posY);
        }

        // （UIManager）に「この文字を、この座標に出せ！」と命令する
        uiManager.CreateBubble(currentFullText, pos);

        currentLine++;
    }

    private void ExecuteRoll(string statName, string successLabel, string failLabel)
    {
        diceManager.ExecuteDiceRoll(statName, successLabel, failLabel, this);

    }

    private void ShowChoices()
    {
        isWaitingForChoice = true;

        uiManager.SetChoicePanelActive(true); // パネル表示
        uiManager.ResetAllChoices();          // ボタン全リセット

        int btnIndex = 0;
        while (currentLine < scenarioLoader.scenarioData.Count)
        {
            ScenarioLine data = scenarioLoader.scenarioData[currentLine];
            string cmd = data.command;

            if (cmd == "Choice")
            {
                string targetLabel = data.param1;

                // ⭕ ボタンの設定もUIManagerに命じる
                uiManager.SetupChoiceButton(btnIndex, data.messageText, () => OnChoiceSelected(targetLabel));

                btnIndex++;
                currentLine++;
            }
            else break;
        }
    }

    private void OnChoiceSelected(string labelName)
    {
        uiManager.SetChoicePanelActive(false);
        isWaitingForChoice = false;
        JumpTo(labelName);
    }

    public void JumpTo(string labelName)
    {
        if (scenarioLoader.labelList.ContainsKey(labelName))
        {
            currentLine = scenarioLoader.labelList[labelName];
            ShowNextLine();
        }

    }


    public void OnClickHiddenButton(string targetLabel)
    {
        if(isWaitingForSearch == false) return;
        if (uiManager.IsTyping) return;

        isWaitingForSearch = false;
        JumpTo(targetLabel);

    }

    public void OnClickFinishCharacterMake(string targetLabel)
    {
        statusGroup.SetActive(false);
        darkPanel.SetActive(false);
        JumpTo(targetLabel);
    }

}
