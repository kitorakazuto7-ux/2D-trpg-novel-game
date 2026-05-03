using UnityEngine;
using TMPro;

public class CharacterMakeManager : MonoBehaviour
{
    [Header("配線するUIとデータ")]
    public PlayerStatus playerStatus;
    public TextMeshProUGUI bonusPointText;
    public GameObject rollButton;

    [Header("ステータス表示用テキスト")]
    public TextMeshProUGUI knowledgeText;
    public TextMeshProUGUI explorationText; // 追加：探索力
    public TextMeshProUGUI staminaText;     // 追加：体力
    public TextMeshProUGUI strengthText;    // 追加：腕力
    public TextMeshProUGUI luckText;        // 追加：運

    private int bonusPoints = 0;
    private bool isRolled = false;

    public void RollBonusPoints()
    {
        int dice = Random.Range(1, 7);
        bonusPoints = 60 + (dice * 10);
        isRolled = true;
        rollButton.SetActive(false);
        UpdateUI();
    }

    public void AddStat(string statName)
    {
        if (!isRolled || bonusPoints < 10) return;

        // どの合言葉（statName）が送られてきたかで処理を分ける
        switch (statName)
        {
            case "knowledge":
                if (playerStatus.knowledge < 100) { playerStatus.knowledge += 10; bonusPoints -= 10; }
                break;
            case "exploration":
                if (playerStatus.exploration < 100) { playerStatus.exploration += 10; bonusPoints -= 10; }
                break;
            case "stamina":
                if (playerStatus.stamina < 100) { playerStatus.stamina += 10; bonusPoints -= 10; }
                break;
            case "strength":
                if (playerStatus.strength < 100) { playerStatus.strength += 10; bonusPoints -= 10; }
                break;
            case "luck":
                if (playerStatus.luck < 100) { playerStatus.luck += 10; bonusPoints -= 10; }
                break;
        }
        UpdateUI();
    }

    public void SubStat(string statName)
    {
        if (!isRolled) return;

        switch (statName)
        {
            case "knowledge":
                if (playerStatus.knowledge > 40) { playerStatus.knowledge -= 10; bonusPoints += 10; }
                break;
            case "exploration":
                if (playerStatus.exploration > 40) { playerStatus.exploration -= 10; bonusPoints += 10; }
                break;
            case "stamina":
                if (playerStatus.stamina > 40) { playerStatus.stamina -= 10; bonusPoints += 10; }
                break;
            case "strength":
                if (playerStatus.strength > 40) { playerStatus.strength -= 10; bonusPoints += 10; }
                break;
            case "luck":
                if (playerStatus.luck > 40) { playerStatus.luck -= 10; bonusPoints += 10; }
                break;
        }
        UpdateUI();
    }
    // --- 追加：決定ボタンを押した時の処理 ---
    public void ConfirmCharacter()
    {
        // まだダイスを振っていないなら何もしない
        if (!isRolled) return;

        // ボーナスポイントが余っているなら進めない
        if (bonusPoints > 0)
        {
            Debug.Log("ポイントをすべて割り振ってください！");
            return;
        }

        // 全ての条件をクリアしたら、キャラメイク完了！
        Debug.Log("キャラクター作成完了！探索へ移行します。");

        MessageManagerTrain1 manager = Object.FindFirstObjectByType<MessageManagerTrain1>();
        if (manager != null)
        {
            // エクセルで探索が始まるラベル*start_searchへ飛ぶように指示！
            manager.OnClickFinishCharacterMake("*start_search");
        }

    }
    private void UpdateUI()
    {
        bonusPointText.text = "残りボーナスポイント：" + bonusPoints.ToString();

        if (knowledgeText != null) knowledgeText.text = "知識：" + playerStatus.knowledge.ToString();
        if (explorationText != null) explorationText.text = "探索力：" + playerStatus.exploration.ToString();
        if (staminaText != null) staminaText.text = "体力：" + playerStatus.stamina.ToString();
        if (strengthText != null) strengthText.text = "腕力：" + playerStatus.strength.ToString();
        if (luckText != null) luckText.text = "運：" + playerStatus.luck.ToString();
    }
}