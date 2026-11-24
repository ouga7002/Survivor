using System.Collections.Generic;
using UnityEngine;

public class EventMissionManager : MonoBehaviour
{
    [Header("発生間隔の設定")]
    [Tooltip("最初のイベントが発生するまでの時間（秒）")]
    [SerializeField] private float firstTriggerDelay = 30f;
    [Tooltip("イベント終了後、次のイベントが発生するまでの間隔（秒）")]
    [SerializeField] private float intervalBetweenEvents = 60f;
    [SerializeField] private bool isRandomOrder = true;

    [Header("登録するイベントミッション一覧")]
    [SerializeField] private List<BaseEventMission> availableMissions = new List<BaseEventMission>();

    [Header("参照設定")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private EventMissionUI missionUI;

    private BaseEventMission currentMission;
    private float nextEventTimer;
    private int sequentialIndex = 0;

    void Start()
    {
        nextEventTimer = firstTriggerDelay;

        // プレイヤー参照の自動補完
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerStatus = playerObj.GetComponent<PlayerStatus>();
            }
        }

        // 全ミッションを初期化
        foreach (var mission in availableMissions)
        {
            if (mission != null)
            {
                mission.Initialize(this, playerTransform, playerStatus);
            }
        }
    }

    void Update()
    {
        // 現在ミッション進行中の場合
        if (currentMission != null && currentMission.IsActive)
        {
            currentMission.UpdateMission(Time.deltaTime);
            if (missionUI != null)
            {
                missionUI.UpdateTimer(currentMission.RemainingTime);
            }
            return;
        }

        // 待機中：次のイベントまでのカウントダウン
        if (availableMissions.Count > 0)
        {
            nextEventTimer -= Time.deltaTime;
            if (nextEventTimer <= 0f)
            {
                TriggerNextMission();
            }
        }
    }

    private void TriggerNextMission()
    {
        if (isRandomOrder)
        {
            int randIndex = Random.Range(0, availableMissions.Count);
            currentMission = availableMissions[randIndex];
        }
        else
        {
            currentMission = availableMissions[sequentialIndex];
            sequentialIndex = (sequentialIndex + 1) % availableMissions.Count;
        }

        if (currentMission != null)
        {
            currentMission.StartMission();
            if (missionUI != null)
            {
                missionUI.ShowMissionStart(currentMission.MissionTitle, currentMission.MissionDescription);
            }
        }
    }

    // ミッション終了時にBaseEventMissionから呼ばれる
    public void NotifyMissionResult(bool isSuccess, string resultMessage)
    {
        if (missionUI != null)
        {
            missionUI.ShowMissionResult(resultMessage);
        }

        currentMission = null;
        nextEventTimer = intervalBetweenEvents;
    }
}