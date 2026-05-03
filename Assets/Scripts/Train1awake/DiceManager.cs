using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [Header("プレイヤーのステータスデータ")]
    public PlayerStatus playerStatus;

    // 監督（MessageManager）から依頼される関数
    public void ExecuteDiceRoll(string statName, string successLabel, string failLabel, MessageManagerTrain1 manager)
    {
        // 1. 指定されたステータスの数値を読み取る
        int targetValue = GetStatValue(statName);

        // 2. 1〜100 のダイスを振る！
        int diceResult = Random.Range(1, 101);

        Debug.Log($"【ダイス判定】{statName}：目標値 {targetValue} / 出目 {diceResult}");

        // 3. 判定（出目が目標値以下なら成功！）
        if (diceResult <= targetValue)
        {
            Debug.Log("結果：成功！");
            manager.JumpTo(successLabel); // 成功用のラベルへ飛ぶ
        }
        else
        {
            Debug.Log("結果：失敗...");
            manager.JumpTo(failLabel);    // 失敗用のラベルへ飛ぶ
        }
    }

    // 文字列（エクセルの指示）から、実際のステータス数値を引っ張り出す
    private int GetStatValue(string statName)
    {
        switch (statName)
        {
            case "knowledge": return playerStatus.knowledge;
            case "exploration": return playerStatus.exploration;
            case "stamina": return playerStatus.stamina;
            case "strength": return playerStatus.strength;
            case "luck": return playerStatus.luck;
            default:
                Debug.LogWarning("存在しないステータスが指定されました: " + statName);
                return 50; // 間違っていたらとりあえず50%にしておく
        }
    }
}