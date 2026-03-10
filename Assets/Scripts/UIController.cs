using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Lifeパネル・各Lifeアイコン")]
    public GameObject lifePanel;
    public GameObject[] lifeIcons;
    [Header("GunPowerパネル・Gunアイコン")]
    public GameObject gunPanel;
    public GameObject gunIcon;
    [Header("銃の画像（配列）")]
    public Sprite[] gunSprites;

    [Header("バレット/マガジンパネル")]
    public GameObject bulletPanel;
    public GameObject magazinePanel;

    [Header("テキスト（スコア・ブレット残弾・マガジン残数）")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI magazineText;

    [Header("リロードパネル")]
    public GameObject reloadPanel;

    [Header("ゲームオーバーパネル・リザルトパネル")]
    public GameObject gameOverPanel;
    public GameObject resultPanel;

    [Header("Playerオブジェクト")]
    public GameObject player;

    Coroutine reloadEndCol; //リロード発生中のコルーチン

    void Start()
    {
        //各UIの更新
        UpdateLife(player.GetComponent<PlayerRun>().Life());
        UpdateBullet();
        UpdateMagazine();
        UpdateScore(ScoreManager.score);
    }

    //体力の更新
    public void UpdateLife(int life)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < life) lifeIcons[i].SetActive(true);
            else lifeIcons[i].SetActive(false);
        }
    }

    //銃の威力の更新
    public void UpdateGun()
    {
        int gunNum = player.GetComponent<NormalShooter>().GetShootPower() - 1;
        if (gunNum < 0) gunNum = 0;
        gunIcon.GetComponent<Image>().sprite = gunSprites[gunNum];
    }

    //残弾の更新
    public void UpdateBullet()
    {
        bulletText.text = player.GetComponent<BulletManager>().GetBulletRemaining().ToString();
    }

    //残マガジンの更新
    public void UpdateMagazine()
    {
        magazineText.text = player.GetComponent<BulletManager>().GetMagazineRemaining().ToString();
    }

    //スコアの更新
    public void UpdateScore(int value)
    {
        scoreText.text = value.ToString();
    }

    //リロード点滅
    public void Reloading()
    {
        //リロードコルーチンの発生
        reloadEndCol = StartCoroutine(ReloadEndCol());
    }

    //リロードコルーチン
    IEnumerator ReloadEndCol()
    {
        yield return new WaitForSeconds(1.0f); //1秒間はリロード点滅
        reloadPanel.SetActive(false); //リロードが終わったら明確にリロードパネルを非表示にしておく
        reloadEndCol = null; //コルーチンの解放
    }

    void Update()
    {
        //ステータスがゲームオーバー
        if (GameManager.gameState == GameState.gameover)
        {
            //余計なものは非表示
            gunPanel.SetActive(false);
            bulletPanel.SetActive(false);
            magazinePanel.SetActive(false);
            //ゲームオーバーパネルの表示
            gameOverPanel.SetActive(true);
            //ゲームステータスをリトライに
            GameManager.gameState = GameState.retry;
        }
        //ステータスがステージクリア
        else if (GameManager.gameState == GameState.stageclear)
        {
            //余計なものは非表示
            gunPanel.SetActive(false);
            bulletPanel.SetActive(false);
            magazinePanel.SetActive(false);
            //リザルトパネルの表示
            resultPanel.SetActive(true);
            //ゲームステータスをリザルトに
            GameManager.gameState = GameState.result;
        }
        //プレイ中にリロードコルーチンが発生していたら
        else if (reloadEndCol != null)
        {
            //点滅で充填中であることを表示
            float val = Mathf.Sin(Time.time * 50); //Sin関数でプラスマイナスの反復値をつくる
            if (val > 0) reloadPanel.SetActive(true); //プラスならリロードパネル表示
            else reloadPanel.SetActive(false); //マイナスならリロードパネル非表示
        }

    }
}
