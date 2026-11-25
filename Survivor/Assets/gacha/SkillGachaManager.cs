
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
    [SerializeField] private Image resultSkillImage;

    [Header("プレイヤー設定")]
    [SerializeField] private GameObject playerObject;

    [Header("スキルデータ設定")]
    [SerializeField]
    private List<SkillData> skillDatabase = new List<SkillData>();

    private GachaState currentState = GachaState.None;

    // スキルIDとスキルコンポーネントの対応表
    private Dictionary<string, ISkillUnlockable> skillLookup =
        new Dictionary<string, ISkillUnlockable>();

    // 獲得済みスキル
    private HashSet<string> ownedSkillIds =
        new HashSet<string>();

    // =====================================
    // 初期設定
    // =====================================

    void Awake()
    {
        FindPlayer();

        BuildSkillLookup();

        ResetAllSkills();

        UpdateUIState(GachaState.None);
    }

    // =====================================
    // プレイヤー取得
    // =====================================

    void FindPlayer()
    {
        if (playerObject != null) return;

        playerObject = GameObject.Find("player2_0");

        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
        }

        if (playerObject == null)
        {
            Debug.LogError(
                "[SkillGachaManager] Playerが見つかりません。"
            );
        }
    }

    // =====================================
    // スキル自動登録
    // =====================================

    void BuildSkillLookup()
    {
        skillLookup.Clear();

        if (playerObject == null) return;

        MonoBehaviour[] components =
            playerObject.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            ISkillUnlockable skill =
                component as ISkillUnlockable;

            if (skill == null) continue;

            if (skillLookup.ContainsKey(skill.SkillId))
            {
                Debug.LogWarning(
                    "[Gacha] スキルIDが重複しています: "
                    + skill.SkillId
                );

                continue;
            }

            skillLookup.Add(skill.SkillId, skill);

            Debug.Log(
                "[Gacha] スキル登録: " + skill.SkillId
            );
        }
    }

    // =====================================
    // スキル初期化
    // =====================================

    void ResetAllSkills()
    {
        ownedSkillIds.Clear();

        foreach (var skill in skillLookup.Values)
        {
            skill.SetSkillUnlocked(false);
        }
    }

    // =====================================
    // 更新処理
    // =====================================

    void Update()
    {
        switch (currentState)
        {
            case GachaState.None:

                if (Input.GetKeyDown(KeyCode.Y) ||
                    Input.GetButtonDown("Fire3"))
                {
                    UpdateUIState(GachaState.GachaScreen);
                }

                break;

            case GachaState.GachaScreen:

                if (Input.GetKeyDown(KeyCode.B) ||
                    Input.GetButtonDown("Fire2"))
                {
                    DrawGacha();
                }
                else if (
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetButtonDown("Fire1") ||
                    Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.None);
                }

                break;

            case GachaState.ResultScreen:

                if (
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetButtonDown("Fire1") ||
                    Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateUIState(GachaState.None);
                }

                break;
        }
    }

    // =====================================
    // UI切り替え
    // =====================================

    void UpdateUIState(GachaState newState)
    {
        currentState = newState;

        if (hudPromptUI != null)
        {
            hudPromptUI.SetActive(
                currentState == GachaState.None
            );
        }

        if (gachaMainUI != null)
        {
            gachaMainUI.SetActive(
                currentState == GachaState.GachaScreen
            );
        }

        if (skillResultUI != null)
        {
            skillResultUI.SetActive(
                currentState == GachaState.ResultScreen
            );
        }

        Time.timeScale =
            currentState == GachaState.None ? 1f : 0f;
    }

    // =====================================
    // ガチャ抽選
    // =====================================

    void DrawGacha()
    {
        List<SkillData> availableSkills =
            new List<SkillData>();

        float totalWeight = 0f;

        // 未獲得スキルを抽選候補に登録
        foreach (SkillData data in skillDatabase)
        {
            if (data == null) continue;

            if (string.IsNullOrEmpty(data.skillId))
                continue;

            if (ownedSkillIds.Contains(data.skillId))
                continue;

            if (data.drawWeight <= 0f)
                continue;

            if (!skillLookup.ContainsKey(data.skillId))
            {
                Debug.LogWarning(
                    "[Gacha] Playerにスキルがありません: "
                    + data.skillId
                );

                continue;
            }

            availableSkills.Add(data);

            totalWeight += data.drawWeight;
        }

        // 全スキル獲得済み、または抽選可能なスキルがない
        if (availableSkills.Count == 0 || totalWeight <= 0f)
        {
            if (resultText != null)
            {
                resultText.text =
                    "すべてのスキルを獲得済み！";
            }

            if (resultSkillImage != null)
            {
                resultSkillImage.gameObject.SetActive(false);
            }

            UpdateUIState(GachaState.ResultScreen);

            return;
        }

        // 重み付きランダム抽選
        float randomValue =
            Random.Range(0f, totalWeight);

        float currentWeight = 0f;

        SkillData selectedSkill = null;

        foreach (SkillData data in availableSkills)
        {
            currentWeight += data.drawWeight;

            if (randomValue < currentWeight)
            {
                selectedSkill = data;
                break;
            }
        }

        // 選択されなかった場合の保険
        if (selectedSkill == null)
        {
            selectedSkill =
                availableSkills[availableSkills.Count - 1];
        }

        AcquireSkill(selectedSkill);

        UpdateUIState(GachaState.ResultScreen);
    }

    // =====================================
    // スキル獲得
    // =====================================

    void AcquireSkill(SkillData data)
    {
        if (data == null) return;

        ISkillUnlockable skill;

        if (!skillLookup.TryGetValue(
            data.skillId, out skill))
        {
            Debug.LogError(
                "[Gacha] スキルが見つかりません: "
                + data.skillId
            );

            return;
        }

        // 獲得済み登録
        ownedSkillIds.Add(data.skillId);

        // スキル解放
        skill.SetSkillUnlocked(true);

        // 結果画面の画像設定
        if (resultSkillImage != null)
        {
            resultSkillImage.gameObject.SetActive(true);

            resultSkillImage.sprite = data.skillIcon;
        }

        // 結果画面のテキスト設定
        if (resultText != null)
        {
            resultText.text =
                data.skillName + " を獲得した！";
        }

        Debug.Log(
            "[ガチャ結果] "
            + data.skillName
            + " を獲得！"
        );
    }

    // =====================================
    // スキル獲得確認
    // =====================================

    public bool HasSkill(string skillId)
    {
        return ownedSkillIds.Contains(skillId);
    }

    // =====================================
    // 獲得済みスキル数
    // =====================================

    public int GetOwnedSkillCount()
    {
        return ownedSkillIds.Count;
    }
}
