
public interface ISkillUnlockable
{
    // スキルを識別するID
    string SkillId { get; }

    // スキルの解放・ロック
    void SetSkillUnlocked(bool unlocked);
}
