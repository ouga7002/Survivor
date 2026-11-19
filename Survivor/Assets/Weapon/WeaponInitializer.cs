using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    [Header("ナイフと銃のオブジェクト（子オブジェクトをアタッチ）")]
    [SerializeField] private GameObject knifeObject; // ヒエラルキーの "Knife 1"
    [SerializeField] private GameObject gunObject;   // ヒエラルキーの "Gun"

    void Start()
    {
        // ゲーム開始時にも一旦現在の設定を適用
        ApplyWeapon();
    }

    // ★武器選択が確定した瞬間に外部から呼び出すメソッド
    public void ApplyWeapon()
    {
        KnifeAttack knifeAttack = GetComponent<KnifeAttack>();
        GunAttack gunAttack = GetComponent<GunAttack>();

        string chosenWeapon = WeaponSelectManager.SelectedWeapon;
        Debug.Log($"【武器適用】選択された武器: {chosenWeapon}");

        if (chosenWeapon == "Gun")
        {
            // 銃を有効化、ナイフを停止
            if (gunObject != null) gunObject.SetActive(true);
            if (gunAttack != null) gunAttack.enabled = true;

            if (knifeObject != null) knifeObject.SetActive(false);
            if (knifeAttack != null) knifeAttack.enabled = false;
        }
        else
        {
            // ナイフを有効化、銃を停止
            if (knifeObject != null) knifeObject.SetActive(true);
            if (knifeAttack != null) knifeAttack.enabled = true;

            if (gunObject != null) gunObject.SetActive(false);
            if (gunAttack != null) gunAttack.enabled = false;
        }
    }
}