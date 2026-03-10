using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score; //ステージスコア ※ゲームオーバー時にリセット
    public static int totalScore = 0; //トータルスコア ※ゲームクリア時に確定
    bool calculated;  //トータル更新済みフラグ

    void Awake()
    {
        score = totalScore; //ステージ開始時点でのスコアの更新
        calculated = false; //必要な計算処理の更新フラグ
    }

    //現ステージスコアアップ
    static public void ScoreUp(int value)
    {        
        score += value;
        //UIの更新
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateScore(score);
    } 

    //現ステージスコアの取得
    static public int GetScore()
    {
        return score;
    }

    void Update()
    {
        //未更新でリトライならステージスコアをもとに戻す
        if (GameManager.gameState == GameState.retry && !calculated)
        {
            score = totalScore;
            calculated = true; //計算済み
        }
        //未更新でリザルトならトータルスコアを確定
        else if (GameManager.gameState == GameState.result && !calculated) 
        {
            totalScore = score; //トータルスコアの更新
            calculated = true; //計算済み
        }
    }
}
