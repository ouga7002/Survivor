using UnityEngine;

public class WeaponSelectManager : MonoBehaviour
{
    // どの武器が選ばれたかを保持する（staticにすることでシーンをまたいでもデータが消えない）
    // デフォルトは "Knife"
    public static string SelectedWeapon = "Knife";

    // ナイフが選ばれたときに呼ぶメソッド
    public void SelectKnife()
    {
        SelectedWeapon = "Knife";
        Debug.Log("【武器選択】ナイフが選ばれました！");
    }

    // 銃が選ばれたときに呼ぶメソッド
    public void SelectGun()
    {
        SelectedWeapon = "Gun";
        Debug.Log("【武器選択】銃が選ばれました！");
    }
}