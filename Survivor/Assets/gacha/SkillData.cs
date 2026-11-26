
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSkillData",
    menuName = "Game/Skill Data"
)]
public class SkillData : ScriptableObject
{
    [Header("スキル情報")]
    public string skillId;
    public string skillName;
    public Sprite skillIcon;

    [Header("ガチャ設定")]
    [Min(0f)]
    public float drawWeight = 1f;
}
