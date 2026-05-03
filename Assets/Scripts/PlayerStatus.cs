using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // --- 追加：唯一無二の存在にする（シングルトン） ---
    public static PlayerStatus Instance;

    private void Awake()
    {
        // もし世界にまだ自分が存在していなかったら
        if (Instance == null)
        {
            Instance = this;               // 「私が本物です」と宣言する
            DontDestroyOnLoad(gameObject); // シーンを移動しても自分を破壊させない
        }
        else
        {
            Destroy(gameObject);           // もしすでに自分が存在していたら、分身を防ぐために自害する
        }
    }
    // ----------------------------------------------------

    [Header("固定ステータス")]
    public int sanity = 100;
    public int karma = 0;

    [Header("技能ステータス（基礎値40）")]
    public int knowledge = 40;
    public int exploration = 40;
    public int stamina = 40;
    public int strength = 40;
    public int luck = 40;
}