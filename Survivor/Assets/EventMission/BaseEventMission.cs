using UnityEngine;

public abstract class BaseEventMission : MonoBehaviour
{
    [Header("ミッション基本設定")]
    [SerializeField] protected string missionTitle = "緊急ミッション";
    [TextArea]
    [SerializeField] protected string missionDescription = "ミッション内容";
    [SerializeField] protected float timeLimit = 30f;

    [Header("クリア報酬")]
    [SerializeField] protected int rewardGP = 50;
    [SerializeField] protected int rewardScore = 1000;

    protected Transform playerTransform;
    protected PlayerStatus playerStatus;
    protected EventMissionManager manager;

    public bool IsActive { get; protected set; }
    public float RemainingTime { get; protected set; }
    public string MissionTitle => missionTitle;
    public string MissionDescription => missionDescription;

    // マネージャーから初期化される
    public void Initialize(EventMissionManager missionManager, Transform player, PlayerStatus status)
    {
        manager = missionManager;
        playerTransform = player;
        playerStatus = status;
    }

    // ミッション開始
    public virtual void StartMission()
    {
        IsActive = true;
        RemainingTime = timeLimit;
        OnMissionStart();
    }

    // 毎フレームの更新（マネージャーから呼ばれる）
    public virtual void UpdateMission(float deltaTime)
    {
        if (!IsActive) return;

        RemainingTime -= deltaTime;

        // 固有の更新・クリア判定
        OnMissionUpdate();

        // まだクリアしておらず、時間切れになったら失敗
        if (IsActive && RemainingTime <= 0f)
        {
            FailMission();
        }
    }

    // クリア処理
    protected void CompleteMission()
    {
        if (!IsActive) return;
        IsActive = false;

        // 報酬付与
        if (playerStatus != null)
        {
            playerStatus.AddGatyaPoint(rewardGP);
            playerStatus.AddScore(rewardScore);
        }

        OnMissionEnd();
        manager.NotifyMissionResult(true, $"【ミッションクリア！】GP +{rewardGP} 獲得！");
    }

    // 失敗処理
    protected void FailMission()
    {
        if (!IsActive) return;
        IsActive = false;

        OnMissionEnd();
        manager.NotifyMissionResult(false, "【ミッション失敗...】時間切れ！");
    }

    // --- 各イベント固有で実装する処理 ---
    protected abstract void OnMissionStart();
    protected abstract void OnMissionUpdate();
    protected abstract void OnMissionEnd();
}