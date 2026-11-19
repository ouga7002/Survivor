using UnityEngine;

public class WeaponSelectUI : MonoBehaviour
{
    [Header("パネルの枠（ハイライト用）オブジェクト")]
    [SerializeField] private GameObject knifeHighlight;
    [SerializeField] private GameObject gunHighlight;

    [Header("武器選択UI全体のパネル（決定後に消す用）")]
    [SerializeField] private GameObject weaponSelectPanel;

    private int selectedIndex = 0;
    private bool isSelecting = true;

    void Start()
    {
        Time.timeScale = 0f;
        isSelecting = true;

        if (weaponSelectPanel != null) weaponSelectPanel.SetActive(true);
        UpdateHighlight();
    }

    void Update()
    {
        if (!isSelecting) return;

        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (horizontal < -0.5f && !isAxisInUse))
        {
            selectedIndex = 0;
            UpdateHighlight();
            StartCoroutine(AxisCooldown());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (horizontal > 0.5f && !isAxisInUse))
        {
            selectedIndex = 1;
            UpdateHighlight();
            StartCoroutine(AxisCooldown());
        }

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

        // ★決定したその瞬間に、プレイヤーの武器を即時切り替える！
        WeaponInitializer initializer = FindFirstObjectByType<WeaponInitializer>();
        if (initializer != null)
        {
            initializer.ApplyWeapon();
        }

        if (weaponSelectPanel != null)
        {
            weaponSelectPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private bool isAxisInUse = false;
    System.Collections.IEnumerator AxisCooldown()
    {
        isAxisInUse = true;
        yield return new WaitForSecondsRealtime(0.2f);
        isAxisInUse = false;
    }
}