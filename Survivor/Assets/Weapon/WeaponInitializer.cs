using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    [Header("ナイフと銃のオブジェクト（子オブジェクトをアタッチ）")]
    [SerializeField] private GameObject knifeObject; // ヒエラルキーの "Knife 1"
    [SerializeField] private GameObject gunObject;   // ヒエラルキーの "Gun"

    void Start()
    {
        // プレイヤー（自分自身）に付いている各攻撃スクリプトを取得
        KnifeAttack knifeAttack = GetComponent<KnifeAttack>();
        GunAttack gunAttack = GetComponent<GunAttack>();

        string chosenWeapon = WeaponSelectManager.SelectedWeapon;
        Debug.Log($"【初期化】選択された武器: {chosenWeapon}");

        if (chosenWeapon == "Gun")
        {
            // === 銃を選んだ場合 ===
            // 1. 銃のオブジェクトを表示し、スクリプトを有効化する
            if (gunObject != null) gunObject.SetActive(true);
            if (gunAttack != null) gunAttack.enabled = true;

            // 2. ナイフのオブジェクトを非表示にし、スクリプトを完全に停止する
            if (knifeObject != null) knifeObject.SetActive(false);
            if (knifeAttack != null) knifeAttack.enabled = false;

            Debug.Log("【初期化】銃モードで起動しました");
        }
        else
        {
            // === ナイフを選んだ場合 ===
            // 1. ナイフのオブジェクトを表示し、スクリプトを有効化する
            if (knifeObject != null) knifeObject.SetActive(true);
            if (knifeAttack != null) knifeAttack.enabled = true;

            // 2. 銃のオブジェクトを非表示にし、スクリプトを完全に停止する
            if (gunObject != null) gunObject.SetActive(false);
            if (gunAttack != null) gunAttack.enabled = false;

            Debug.Log("【初期化】ナイフモードで起動しました");
        }
    }
}