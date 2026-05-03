using UnityEngine;
using UnityEngine.SceneManagement; // ワープの呪文の準備

public class TitleManager : MonoBehaviour
{
    // 「はじめから」ボタンを押した時に発動する魔法
    public void GameStart()
    {
        // 最初のシナリオ（プロローグ）のシーンへワープする
        SceneManager.LoadScene("PrologueScene");
    }
}