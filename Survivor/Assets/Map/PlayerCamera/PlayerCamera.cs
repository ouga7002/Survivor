using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従するターゲット (Player)")]
    public Transform target;

    [Header("カメラのオフセット (Z軸は通常-10などマイナスにする)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("追従の滑らかさ (数値が小さいほど滑らか、0で完全に固定)")]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        // ターゲットの位置にオフセットを足した目標位置を計算
        Vector3 desiredPosition = target.position + offset;

        // 現在の位置から目標の位置へ滑らかに移動させる（カクつきをなくす場合）
        // 完全にカチッと固定したい場合は、transform.position = desiredPosition; にしてください
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Z軸はカメラの奥行きを維持するため、targetのZではなくoffsetのZを適用
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, offset.z);
    }
}