using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必要

public class ResultManager : MonoBehaviour
{
    // タイトル画面へ戻るための関数
    public void GoToTitle()
    {
        // "TitleScene" の部分は、実際のタイトル画面のシーン名に書き換えてください
        SceneManager.LoadScene("TitleScene");
    }
}