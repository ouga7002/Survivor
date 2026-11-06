using UnityEngine;

public enum OrbEffectType
{
    Experience,
    Heal,
    AttackUp
}

[CreateAssetMenu(
    fileName = "NewOrbData",
    menuName = "Game/Orb Data"
)]
public class OrbData : ScriptableObject
{
    [Header("オーブ設定")]
    public string orbName;

    public GameObject orbPrefab;

    [Header("効果設定")]
    public OrbEffectType effectType;

    public float effectAmount = 1f;

    [Tooltip("攻撃力アップの持続時間")]
    public float duration = 5f;
}