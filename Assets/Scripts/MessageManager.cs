
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    // ⭕ 監督が使う「連絡先」はこの2つだけ！
    [SerializeField] private ScenarioLoader scenarioLoader;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject DiffImage;
    [SerializeField] private GameObject searchButtonGroup;

    private int currentLine = 0;
    private bool isWaitingForChoice = false;
    private bool isWaitingForSearch = false;
    private bool OnClickHiddenItemPopup = false;
    private bool OnClickHiddenItemHuman = false;
    private bool OnClickHiddenItemAir = false;
    private string currentFullText = "";
    public GameObject phoneButton;

    void Start()
    {
        // UIの初期化（全部消す）

        phoneButton.SetActive(false);
        uiManager.SetChoicePanelActive(false);
        searchButtonGroup.SetActive(false);
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
            // シナリオ終了のUIもUIManagerに任せる（座標は適当に中央など）
            uiManager.CreateBubble("（シナリオ終了）", Vector2.zero);
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
            searchButtonGroup.SetActive(true);
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
        // 変更なし（以前のまま）
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

    private void JumpTo(string labelName)
    {
        if (scenarioLoader.labelList.ContainsKey(labelName))
        {
            currentLine = scenarioLoader.labelList[labelName];
            ShowNextLine();
        }
       
    }

    public void OnClickHiddenItem(string targetLabel)
    {

        searchButtonGroup.SetActive(false);


        // ▼ 追加：今なんのラベルが押されたか、コンソールに証拠を出す！
        Debug.Log("【証拠】今クリックされたラベルはコレだ！👉 [" + targetLabel + "]");

        if (targetLabel == "*anyhuman") OnClickHiddenItemHuman = true;
        if (targetLabel == "*pop-up") OnClickHiddenItemPopup = true;
        if (targetLabel == "*air") OnClickHiddenItemAir = true;

        // ▼ 追加：今の3つのアイテムの発見状況をコンソールに出す！
        Debug.Log($"【状況】Human:{OnClickHiddenItemHuman} / Popup:{OnClickHiddenItemPopup} / Air:{OnClickHiddenItemAir}");

        if (OnClickHiddenItemHuman && OnClickHiddenItemPopup && OnClickHiddenItemAir)
        {
            Debug.Log("🌟 3つ全部trueになったから画像を出すよ！！");
            DiffImage.SetActive(true);
            phoneButton.SetActive(true);
        }

        isWaitingForSearch = false;
        JumpTo(targetLabel);
    }

    public void OnClickPhoneButton()
    {
        isWaitingForSearch = false;
        phoneButton.SetActive(false);
        JumpTo("*Phone");

    }

}