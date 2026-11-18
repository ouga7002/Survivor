using UnityEngine;

public class WeaponSelectUI : MonoBehaviour
{
    [Header("パネルの枠（ハイライト用）オブジェクト")]
    [SerializeField] private GameObject knifeHighlight; // ナイフ側の光る枠
    [SerializeField] private GameObject gunHighlight;   // 銃側の光る枠

    [Header("武器選択UI全体のパネル（決定後に消す用）")]
    [SerializeField] private GameObject weaponSelectPanel; // 選択画面全体の親パネル

    // 0 = ナイフ選択中, 1 = 銃選択中
    private int selectedIndex = 0;
    private bool isSelecting = true; // 選択中かどうか

    void Start()
    {
        // 1. ゲーム開始時は時間を完全に止める（ポーズ状態にする）
        Time.timeScale = 0f;
        isSelecting = true;

        // 2. 選択UIを表示し、ハイライトを初期化
        if (weaponSelectPanel != null) weaponSelectPanel.SetActive(true);
        UpdateHighlight();
    }

    void Update()
    {
        // 選択が終わっている時は入力を受け付けない
        if (!isSelecting) return;

        float horizontal = Input.GetAxisRaw("Horizontal");

        // 左入力（ナイフ）
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (horizontal < -0.5f && !isAxisInUse))
        {
            selectedIndex = 0;
            UpdateHighlight();
            StartCoroutine(AxisCooldown());
        }
        // 右入力（銃）
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (horizontal > 0.5f && !isAxisInUse))
        {
            selectedIndex = 1;
            UpdateHighlight();
            StartCoroutine(AxisCooldown());
        }

        // 決定ボタン（Aボタン / Space / Enter等）
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    void UpdateHighlight()
    {
        if (knifeHighlight != null) knifeHighlight.SetActive(selectedIndex == 0);
        if (gunHighlight != null) gunHighlight.SetActive(selectedIndex == 1);
    }

    void ConfirmSelection()
    {
        isSelecting = false;

        // 選択された武器をマネージャーに保存
        if (selectedIndex == 0)
        {
            WeaponSelectManager.SelectedWeapon = "Knife";
            Debug.Log("【武器選択】ナイフで決定！ゲームスタート");
        }
        else
        {
            WeaponSelectManager.SelectedWeapon = "Gun";
            Debug.Log("【武器選択】銃で決定！ゲームスタート");
        }

        // 3. 選択UIパネルを非表示にする
        if (weaponSelectPanel != null)
        {
            weaponSelectPanel.SetActive(false);
        }

        // 4. ゲームの時間を通常に戻し、プレイを開始する
        Time.timeScale = 1f;

        // ※もし、武器の切り替え判定をここで即座に行いたい場合は、
        // プレイヤーの WeaponInitializer に再適用を促すこともできますが、
        // プレイヤー側の Start() がこの後（またはすで）に動くため、そのままで基本は機能します。
    }

    private bool isAxisInUse = false;
    System.Collections.IEnumerator AxisCooldown()
    {
        isAxisInUse = true;
        yield return new WaitForSecondsRealtime(0.2f); // ポーズ中でも動くリアルタイム待機
        isAxisInUse = false;
    }
}