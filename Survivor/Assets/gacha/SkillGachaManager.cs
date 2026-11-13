using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("UIテキスト・画像設定")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image resultSkillImage; // 結果画面のアイコン表示用Image

    [Header("スキルのアイコン画像（Sprite）")]
    [SerializeField] private Sprite shurikenIcon;
    [SerializeField] private Sprite lightningIcon;
    [SerializeField] private Sprite vampireIcon;

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

    void ResetAllSkills()
    {
        HasShuriken = false;
        HasLightning = false;
        HasVampire = false;

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
            if (resultSkillImage != null) resultSkillImage.gameObject.SetActive(false); // 画像を隠すなど
            Debug.Log("【ガチャ】すべてのスキルを獲得済み！");
            UpdateUIState(GachaState.ResultScreen);
            return;
        }

        int randomIndex = Random.Range(0, unownedSkills.Count);
        int selectedSkillID = unownedSkills[randomIndex];

        string acquiredName = AcquireSkill(selectedSkillID);

        Debug.Log($"【ガチャ結果】 {acquiredName} を獲得した！");
        UpdateUIState(GachaState.ResultScreen);
    }

    string AcquireSkill(int skillID)
    {
        GameObject playerObj = GetPlayerObject();

        if (resultSkillImage != null) resultSkillImage.gameObject.SetActive(true); // 画像を表示状態にする

        switch (skillID)
        {
            case 0:
                HasShuriken = true;
                if (playerObj != null)
                {
                    Shuriken shuriken = playerObj.GetComponent<Shuriken>();
                    if (shuriken != null) shuriken.isUnlocked = true;
                }
                // ★手裏剣の画像をセット＆テキスト変更
                if (resultSkillImage != null) resultSkillImage.sprite = shurikenIcon;
                if (resultText != null) resultText.text = "手裏剣 を獲得した！";
                return "手裏剣";

            case 1:
                HasLightning = true;
                if (playerObj != null)
                {
                    LightningSkill lightning = playerObj.GetComponent<LightningSkill>();
                    if (lightning != null) lightning.isUnlocked = true;
                }
                // ★雷撃の画像をセット＆テキスト変更
                if (resultSkillImage != null) resultSkillImage.sprite = lightningIcon;
                if (resultText != null) resultText.text = "雷撃 を獲得した！";
                return "雷撃";

            case 2:
                HasVampire = true;
                if (playerObj != null)
                {
                    VampireSkill vampire = playerObj.GetComponent<VampireSkill>();
                    if (vampire != null) vampire.isUnlocked = true;
                }
                // ★吸血の画像をセット＆テキスト変更
                if (resultSkillImage != null) resultSkillImage.sprite = vampireIcon;
                if (resultText != null) resultText.text = "吸血 を獲得した！";
                return "吸血";

            default:
                return "";
        }
    }

    GameObject GetPlayerObject()
    {
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null) return pObj;

        return GameObject.FindWithTag("Player");
    }
}