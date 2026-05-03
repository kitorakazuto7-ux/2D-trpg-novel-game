using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("吹き出しUI")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private Transform bubbleParent;

    [Header("選択肢UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("文字送り設定")]
    [SerializeField] private float typeSpeed = 0.05f;

    private GameObject currentBubble;
    private TextMeshProUGUI currentBubbleText;
    private Coroutine typeCoroutine;
    public bool IsTyping { get; private set; } // 監督が「今、文字打ち中かな？」と確認するための窓口

    // ① 吹き出しを新しく作る・座標を動かす
    public void CreateBubble(string text, Vector2 position)
    {
        if (currentBubble != null) Destroy(currentBubble);

        currentBubble = Instantiate(bubblePrefab, bubbleParent);
        RectTransform rect = currentBubble.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        currentBubbleText = currentBubble.GetComponentInChildren<TextMeshProUGUI>();

        // 文字打ち開始
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypeText(text, currentBubbleText));
    }

    // ② 吹き出しを消すだけ
    public void ClearBubble()
    {
        if (currentBubble != null) Destroy(currentBubble);
    }

    // ③ 文字送りを強制終了（スキップ）する
    public void FinishTyping(string fullText)
    {
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        currentBubbleText.text = fullText;
        IsTyping = false;
    }

    // ④ 選択肢パネルの表示・非表示
    public void SetChoicePanelActive(bool active)
    {
        choicePanel.SetActive(active);
    }

    // ⑤ 選択肢ボタンのリセット（全部消す）
    public void ResetAllChoices()
    {
        foreach (var btn in choiceButtons) btn.gameObject.SetActive(false);
    }

    // ⑥ 特定のボタンにテキストと機能をセットする
    public void SetupChoiceButton(int index, string text, System.Action onClickAction)
    {
        if (index < choiceButtons.Length)
        {
            choiceButtons[index].gameObject.SetActive(true);
            choiceTexts[index].text = text;
            choiceButtons[index].onClick.RemoveAllListeners();
            choiceButtons[index].onClick.AddListener(() => onClickAction());
        }
    }

    // 裏方の文字打ち処理
    private IEnumerator TypeText(string text, TextMeshProUGUI targetText)
    {
        IsTyping = true;
        targetText.text = "";
        foreach (char c in text)
        {
            targetText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        IsTyping = false;
    }

}