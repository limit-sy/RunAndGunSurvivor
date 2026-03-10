using UnityEngine;
using UnityEngine.SceneManagement;

//ゲームステータスの定義
public enum GameState
{
    none,
    title,
    demo,
    gameplay,
    gameover,
    retry, //gameover→retryに移行
    stageclear,
    result,//stageclear→resultに移行
    gameclear
}

public class GameManager : MonoBehaviour
{
    public static GameState gameState = GameState.none;

    [Header("ステージクリア時の次シーン")]
    public string nextScene;

    void Start()
    {
        //ゲーム状態をgameplayに
        gameState = GameState.gameplay;
    }

    //リトライ（現シーン名を取得）
    static public void RetryScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //次シーンへ
    public void NextScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
}
