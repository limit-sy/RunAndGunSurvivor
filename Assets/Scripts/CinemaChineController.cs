using UnityEngine;
using UnityEngine.Playables;

public class CinemaChineController : MonoBehaviour
{
    //Timelineをコントロールするコンポーネント
    PlayableDirector playableDirector;

    [Header("リザルト時・リトライ時のタイムライン")]
    public PlayableAsset gameResult; //ゲームクリア時のTimeline
    public PlayableAsset gameRetry; //ゲームオーバー時のTimeline

    bool isPlaying; //バーチャルカメラの作動フラグ

    void Start()
    {
        //コンポーネントを取得するが、タイミングがくるまでは無効化
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.enabled = false;
    }

    void Update()
    {
        //まだバーチャルカメラ作動前でリザルトになったら
        if(!isPlaying && GameManager.gameState == GameState.result)
        {
            playableDirector.enabled = true; //コンポーネントの有効化
            playableDirector.Stop(); //とりあえずストップ
            playableDirector.playableAsset = gameResult; //どのタイムラインを使うのかセットアップ
            playableDirector.Play(); //タイムラインの再生
            isPlaying = true; //バーチャルカメラの作動フラグをON
        }
        
        //まだバーチャルカメラ作動前でリトライになったら
        if (!isPlaying && GameManager.gameState == GameState.retry)
        {
            playableDirector.enabled = true; //コンポーネントの有効化
            playableDirector.Stop(); //とりあえずストップ
            playableDirector.playableAsset = gameRetry; //どのタイムラインを使うのかセットアップ
            playableDirector.Play(); //タイムラインの再生
            isPlaying = true; //バーチャルカメラの作動フラグをON
        }
    }
}
