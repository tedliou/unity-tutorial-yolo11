using MDF.YOLO11;
using UnityEngine;

// 將方塊 (Cube)、圓球 (Sphere)、圓柱體 (Cylinder) 分別移動到
// YOLO 偵測到的第一個人的鼻子、左手腕、右手腕的關節位置
public class CustomManager : MonoBehaviour
{
    // 公開變數
    // 鼻子 (Cube)、左手腕 (Sphere)、右手腕 (Cylinder)
    public GameObject nose, leftWrist, rightWrist;

    // 遊戲開始時監聽 BodyTracker 的更新事件
    // UpdateCubes 被呼叫時代表 YOLO 有順利追蹤到肢體
    private void Start()
    {
        GetComponent<BodyTracker>().ValueChanged += UpdateCubes;
    }

    // YOLO 追蹤到肢體時會一直呼叫這個
    // 分別從肢體追蹤的結果中取出座標，並移動物件到指定座標 (關節的座標)
    // COCOKeypoint.XXXX 是關節的名稱
    private void UpdateCubes(BodyKeypoint keypoints)
    {
        UpdateJointObject(keypoints, COCOKeypoint.Nose, nose);
        UpdateJointObject(keypoints, COCOKeypoint.LeftWrist, leftWrist);
        UpdateJointObject(keypoints, COCOKeypoint.RightWrist, rightWrist);
    }

    // 取出第一個人的指定關節座標，並轉換成世界座標
    // 讓物件在遊戲畫面上的位置等同於 YOLO 看到的位置
    private void UpdateJointObject(BodyKeypoint keypoints, COCOKeypoint joint, GameObject targetObj)
    {
        var point = keypoints[0][joint];

        // 如果有勾選 Body Tracker 的 Invert X 或 Invert Y
        // 沒偵測到的關節有可能會是 0 或 1 的初始值
        // 當座標為初始值時要當作無效座標，將物件隱藏
        if (IsInvisiable(point))
        {
            targetObj.SetActive(false);
            return;
        }

        var screenPos = new Vector3(Screen.width * point.x, Screen.height * point.y, Mathf.Abs(Camera.main.transform.position.z));
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        targetObj.transform.position = worldPos;
        targetObj.SetActive(true);
    }

    // 最好要用 Mathf.Approximately 來比對浮點數的座標
    // 因浮點數偶爾會有誤差，用 == 可能會出 Bug
    private bool IsInvisiable(Vector2 point)
    {
        return
            (Mathf.Approximately(point.x, 1) || Mathf.Approximately(point.x, 0)) &&
            (Mathf.Approximately(point.y, 1) || Mathf.Approximately(point.y, 0));
    }
}
