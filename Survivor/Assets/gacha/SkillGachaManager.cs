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

    [Header("管理対象スキルオブジェクト")]
    [SerializeField] private GameObject shurikenObj;
    [SerializeField] private GameObject lightningObj;
    [SerializeField] private GameObject vampireObj;

    public bool HasShuriken { get; private set; } = false;
    public bool HasLightning { get; private set; } = false;
    public bool HasVampire { get; private set; } = false;

    private GachaState currentState = GachaState.None;

    void Awake()
    {
        ResetSkillsAtAwake();
    }

    void Start()
    {
        ResetSkillsAtAwake();
        UpdateUIState(GachaState.None);
    }

    void ResetSkillsAtAwake()
    {
        HasShuriken = false;
        HasLightning = false;
        HasVampire = false;

        // ゲーム開始時はオブジェクトを非アクティブにし、スクリプトもオフにする
        SetSkillActiveAndEnabled(shurikenObj, false);
        SetSkillActiveAndEnabled(lightningObj, false);
        SetSkillActiveAndEnabled(vampireObj, false);
    }

    void Update()
    {
        switch (currentState)
        {
            case GachaState.None:
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("Fire3"))
                {
                    UpdateUIState(GachaState.GachaScreen);
                }
                break;

            case GachaState.GachaScreen:
                if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2"))
                {
                    DrawGacha();
                }
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
        switch (skillID)
        {
            case 0:
                HasShuriken = true;
                SetSkillActiveAndEnabled(shurikenObj, true); // オブジェクト＋スクリプトを有効化
                return "手裏剣";
            case 1:
                HasLightning = true;
                SetSkillActiveAndEnabled(lightningObj, true); // オブジェクト＋スクリプトを有効化
                return "雷撃";
            case 2:
                HasVampire = true;
                SetSkillActiveAndEnabled(vampireObj, true); // オブジェクト＋スクリプトを有効化
                return "吸血";
            default:
                return "";
        }
    }

    // ★重要：オブジェクトの有効/無効だけでなく、アタッチされているスキルコンポーネントも確実につけ外しするヘルパー
    void SetSkillActiveAndEnabled(GameObject skillObj, bool activeState)
    {
        if (skillObj == null) return;

        // 1. オブジェクト自体の有効/無効を設定
        skillObj.SetActive(activeState);

        // 2. オブジェクトに付いているカスタムスキル用スクリプト（Shuriken, LightningSkill, VampireSkill）を探して一括制御
        MonoBehaviour[] scripts = skillObj.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != null && script != this)
            {
                script.enabled = activeState;
            }
        }
    }
}