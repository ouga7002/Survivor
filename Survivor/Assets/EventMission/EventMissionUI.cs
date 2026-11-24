using System.Collections;
using UnityEngine;
using TMPro;

public class EventMissionUI : MonoBehaviour
{
    [Header("UI要素（TextMeshPro）")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("結果表示の時間（秒）")]
    [SerializeField] private float resultDisplayDuration = 3f;

    private Coroutine hideCoroutine;

    void Start()
    {
        ClearUI();
    }

    public void ShowMissionStart(string title, string description)
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
    }

    public void UpdateTimer(float remainingTime)
    {
        if (timerText != null)
        {
            timerText.text = $"残り: {Mathf.Ceil(remainingTime)}秒";
        }
    }

    public void ShowMissionResult(string message)
    {
        if (titleText != null) titleText.text = message;
        if (descriptionText != null) descriptionText.text = "";
        if (timerText != null) timerText.text = "";

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        ClearUI();
    }

    private void ClearUI()
    {
        if (titleText != null) titleText.text = "";
        if (descriptionText != null) descriptionText.text = "";
        if (timerText != null) timerText.text = "";
    }
}