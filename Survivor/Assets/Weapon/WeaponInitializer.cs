using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    void Start()
    {
        // プレイヤーにアタッチされている各攻撃スクリプトを取得
        KnifeAttack knifeAttack = GetComponent<KnifeAttack>();
        GunAttack gunAttack = GetComponent<GunAttack>();

        string chosenWeapon = WeaponSelectManager.SelectedWeapon;
        Debug.Log($"【初期化】選択されていた武器: {chosenWeapon}");

        if (chosenWeapon == "Gun")
        {
            // 銃が選ばれていた場合：銃を有効化し、ナイフを無効化する
            if (gunAttack != null) gunAttack.enabled = true;
            if (knifeAttack != null) knifeAttack.enabled = false;
        }
        else
        {
            // ナイフが選ばれていた場合（デフォルト）：ナイフを有効化し、銃を無効化する
            if (knifeAttack != null) knifeAttack.enabled = true;
            if (gunAttack != null) gunAttack.enabled = false;
        }
    }
}