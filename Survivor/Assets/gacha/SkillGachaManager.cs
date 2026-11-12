using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillGachaManager : MonoBehaviour
{
    public enum GachaState
    {
        None,
        GachaScreen,
        ResultScreen
    }

    [Header("UIパネル設定")]
    [SerializeField] private GameObject hudPromptUI;
    [SerializeField] private GameObject gachaMainUI;
    [SerializeField] private GameObject skillResultUI;

    [Header("UIテキスト設定")]
    [SerializeField] private TextMeshProUGUI resultText;

    // 所持フラグ（プレイごとにリセットされる）
    public bool HasShuriken { get; private set; } = false;
    public bool HasLightning { get; private set; } = false;
    public bool HasVampire { get; private set; } = false;

    private GachaState currentState = GachaState.None;

    void Awake()
    {
        ResetAllSkills();
        UpdateUIState(GachaState.None);
    }

    void Start()
    {
        UpdateUIState(GachaState.None);
    }

    // 全スキルの所持フラグと、プレイヤー側のスキルをすべてリセット（未取得）にする
    void ResetAllSkills()
    {
        HasShuriken = false;
        HasLightning = false;
        HasVampire = false;

        // プレイヤーを取得して、すべてのスキルの isUnlocked を false にする
        GameObject playerObj = GetPlayerObject();
        if (playerObj != null)
        {
            Shuriken shuriken = playerObj.GetComponent<Shuriken>();
            if (shuriken != null) shuriken.isUnlocked = false;

            LightningSkill lightning = playerObj.GetComponent<LightningSkill>();
            if (lightning != null) lightning.isUnlocked = false;

            VampireSkill vampire = playerObj.GetComponent<VampireSkill>();
            if (vampire != null) vampire.isUnlocked = false;
        }

        Debug.Log("【SkillGachaManager】全スキルの所持状態をリセットしました。");
    }

    void Update()
    {
        switch (currentState)
        {
            case GachaState.None:
                // Yボタンまたはキーボード等でガチャ画面を開く
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("Fire3"))
                {
                    UpdateUIState(GachaState.GachaScreen);
                }
                break;

            case GachaState.GachaScreen:
                // Bボタン等でガチャを引く
                if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2"))
                {
                    DrawGacha();
                }
                // AボタンやESCで閉じる
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.None);
                }
                break;

            case GachaState.ResultScreen:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.None);
                }
                break;
        }
    }

    void UpdateUIState(GachaState newState)
    {
        currentState = newState;

        if (hudPromptUI != null) hudPromptUI.SetActive(currentState == GachaState.None);
        if (gachaMainUI != null) gachaMainUI.SetActive(currentState == GachaState.GachaScreen);
        if (skillResultUI != null) skillResultUI.SetActive(currentState == GachaState.ResultScreen);

        // ガチャ画面中はゲームを一時停止（Noneのときは通常速度）
        Time.timeScale = (currentState == GachaState.None) ? 1f : 0f;
    }

    void DrawGacha()
    {
        List<int> unownedSkills = new List<int>();

        if (!HasShuriken) unownedSkills.Add(0);
        if (!HasLightning) unownedSkills.Add(1);
        if (!HasVampire) unownedSkills.Add(2);

        if (unownedSkills.Count == 0)
        {
            if (resultText != null) resultText.text = "すべてのスキルを獲得済み！";
            Debug.Log("【ガチャ】すべてのスキルを獲得済み！");
            UpdateUIState(GachaState.ResultScreen);
            return;
        }

        int randomIndex = Random.Range(0, unownedSkills.Count);
        int selectedSkillID = unownedSkills[randomIndex];

        string acquiredName = AcquireSkill(selectedSkillID);

        Debug.Log($"【ガチャ結果】 {acquiredName} を獲得した！");

        if (resultText != null)
        {
            resultText.text = $"{acquiredName} を獲得した！";
        }

        UpdateUIState(GachaState.ResultScreen);
    }

    string AcquireSkill(int skillID)
    {
        GameObject playerObj = GetPlayerObject();
        if (playerObj == null)
        {
            Debug.LogError("【ガチャ】プレイヤーが見つかりませんでした！");
            return "";
        }

        switch (skillID)
        {
            case 0:
                HasShuriken = true;
                Shuriken shuriken = playerObj.GetComponent<Shuriken>();
                if (shuriken != null) shuriken.isUnlocked = true;
                return "手裏剣";

            case 1:
                HasLightning = true;
                LightningSkill lightning = playerObj.GetComponent<LightningSkill>();
                if (lightning != null) lightning.isUnlocked = true;
                return "雷撃";

            case 2:
                HasVampire = true;
                VampireSkill vampire = playerObj.GetComponent<VampireSkill>();
                if (vampire != null) vampire.isUnlocked = true;
                return "吸血";

            default:
                return "";
        }
    }

    // プレイヤーオブジェクトを安全に探すヘルパーメソッド
    GameObject GetPlayerObject()
    {
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null) return pObj;

        return GameObject.FindWithTag("Player");
    }
}