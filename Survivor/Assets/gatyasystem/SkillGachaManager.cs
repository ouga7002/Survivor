using UnityEngine;
using TMPro; // TMPを使う場合は有効化

public class SkillGachaManager : MonoBehaviour
{
    // ガチャのステート（現在の画面状態）
    public enum GachaState
    {
        None,         // プレイ中（常時UIのみ表示）
        GachaScreen,  // ガチャ引き画面（Yを押した状態）
        ResultScreen  // スキル獲得結果画面（Bでガチャを引いた状態）
    }

    [Header("UIパネル設定")]
    [Tooltip("画面端の常時UI（Yでガチャを開く案内）")]
    [SerializeField] private GameObject hudPromptUI;

    [Tooltip("ガチャ実行画面パネル")]
    [SerializeField] private GameObject gachaMainUI;

    [Tooltip("スキル獲得結果画面パネル")]
    [SerializeField] private GameObject skillResultUI;

    [Header("結果表示用テキスト（任意）")]
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("排出スキル（ゲームオブジェクト/プレハブ）")]
    [SerializeField] private GameObject shurikenSkillObj;
    [SerializeField] private GameObject lightningSkillObj;
    [SerializeField] private GameObject vampireSkillObj;

    // 所持状況
    public bool HasShuriken { get; private set; } = false;
    public bool HasLightning { get; private set; } = false;
    public bool HasVampire { get; private set; } = false;

    private GachaState currentState = GachaState.None;

    void Start()
    {
        // 初期状態のスキル無効化
        if (shurikenSkillObj != null) shurikenSkillObj.SetActive(false);
        if (lightningSkillObj != null) lightningSkillObj.SetActive(false);
        if (vampireSkillObj != null) vampireSkillObj.SetActive(false);

        // 画面初期化
        UpdateUIState(GachaState.None);
    }

    void Update()
    {
        switch (currentState)
        {
            case GachaState.None:
                // プレイ中：Yボタン（またはYキー）でガチャ画面へ
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("Fire3"))
                {
                    UpdateUIState(GachaState.GachaScreen);
                }
                break;

            case GachaState.GachaScreen:
                // ガチャ画面：Bボタンでガチャを引く
                if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2"))
                {
                    DrawGacha();
                }
                // Aボタン（またはAキー/Esc）で前の画面（プレイ画面）へ戻る
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.None);
                }
                break;

            case GachaState.ResultScreen:
                // 結果画面：Aボタンで前の画面（ガチャ画面）へ戻る
                if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.GachaScreen);
                }
                break;
        }
    }

    // 画面状態を一元管理して表示・非表示を切り替えるメソッド
    void UpdateUIState(GachaState newState)
    {
        currentState = newState;

        // 各パネルの有効・無効を切り替え
        if (hudPromptUI != null) hudPromptUI.SetActive(currentState == GachaState.None);
        if (gachaMainUI != null) gachaMainUI.SetActive(currentState == GachaState.GachaScreen);
        if (skillResultUI != null) skillResultUI.SetActive(currentState == GachaState.ResultScreen);

        // プレイ中以外はゲームを時停止（一時停止させたい場合）
        Time.timeScale = (currentState == GachaState.None) ? 1f : 0f;
    }

    // ガチャ実行
    void DrawGacha()
    {
        System.Collections.Generic.List<int> unownedSkills = new System.Collections.Generic.List<int>();
        if (!HasShuriken) unownedSkills.Add(0);
        if (!HasLightning) unownedSkills.Add(1);
        if (!HasVampire) unownedSkills.Add(2);

        if (unownedSkills.Count == 0)
        {
            if (resultText != null) resultText.text = "全スキル所持済み！";
            UpdateUIState(GachaState.ResultScreen);
            return;
        }

        int randomIndex = Random.Range(0, unownedSkills.Count);
        int selectedSkill = unownedSkills[randomIndex];

        string acquiredName = AcquireSkill(selectedSkill);

        if (resultText != null)
        {
            resultText.text = $"{acquiredName} を獲得！";
        }

        // 抽選結果画面へ遷移
        UpdateUIState(GachaState.ResultScreen);
    }

    string AcquireSkill(int skillID)
    {
        switch (skillID)
        {
            case 0:
                HasShuriken = true;
                if (shurikenSkillObj != null) shurikenSkillObj.SetActive(true);
                return "手裏剣";
            case 1:
                HasLightning = true;
                if (lightningSkillObj != null) lightningSkillObj.SetActive(true);
                return "雷撃";
            case 2:
                HasVampire = true;
                if (vampireSkillObj != null) vampireSkillObj.SetActive(true);
                return "吸血";
            default:
                return "";
        }
    }
}