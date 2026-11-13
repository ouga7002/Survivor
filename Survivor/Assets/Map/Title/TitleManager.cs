using UnityEngine;
using UnityEngine.SceneManagement; // これがシーン切り替えに必要です

public class TitleManager : MonoBehaviour
{
    // ボタンを押したときに呼び出す関数
    public void StartGame()
    {
        // "GameScene" の部分は、移動したいゲーム本編のシーン名に書き換えてください
        SceneManager.LoadScene("SampleScene");
    }
}