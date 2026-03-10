using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("生成プレハブオブジェクト")]
    public GameObject effectPrefab; //生成プレハブ

    [Header("耐久力")]
    public float life = 5.0f; //耐久力

    [Header("ダメージ時間・振動対象・振動スピード・振動量")]
    public float damageTime = 0.25f; //ダメージ中時間
    public GameObject damageBody; //振動対象オブジェクト
    public float speed = 75.0f; //振動スピード
    public float amplitude = 1.5f;  //振動量

    [Header("スコア点数")]
    public int point = 100;

    Vector3 startPosition; //振動対象の初期位置
    float x; //振動による移動座標

    Coroutine currentDamage; //ダメージコルーチン

    void Start()
    {
        // 振動対象となるオブジェクトのローカル初期値を取得
        startPosition = damageBody.transform.localPosition;
    }

    void Update()
    {
        // もしもダメージコルーチンが発動中だったら振動
        if (currentDamage != null)
        {
            // Mathf.Sinで一定間隔でプラスマイナスの値を往復する
            x = (amplitude * 0.01f) * Mathf.Sin(Time.time * speed);
            damageBody.transform.localPosition = startPosition + new Vector3(x, 0, 0);
        }
    }

    //衝突
    void OnTriggerEnter(Collider other)
    {
        if (currentDamage != null) return;  // ダメージコルーチ御中ならキャンセル
        //if (bool型変数がtrue) return; // この書き方でもよい

        // 衝突相手が「Bullet」タグを持っていた場合
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Sword")
        {
            // 相手がどのタグなのかを取得
            string tag = other.gameObject.tag;

            // ダメージコルーチンを発動
            currentDamage = StartCoroutine(DamageCol(tag));
            // lifeが残っていなければ消滅
            if (life <= 0)
            {
                ScoreManager.ScoreUp(point);    // 敵撃破でスコアアップ

                CreateEffect();
            }
        }
    }

    //ダメージコルーチン
    IEnumerator DamageCol(string tag)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //bool型変数をtrue
        if (tag == "Bullet")
        {
            life -= player.GetComponent<NormalShooter>().GetShootPower();
        }
        else if (tag == "Sword")
        {
            life -= player.GetComponent<NormalSword>().GetSwordPower();
        }

        yield return new WaitForSeconds(damageTime);
        // コルーチンを発動していたという情報の解除
        currentDamage = null;
        //bool型変数をfalseにする

        yield return new WaitForSeconds(0.1f);
        //振動していたボディを元の位置に戻す
        damageBody.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void CreateEffect()
    {
        // もしエフェクトプレハブが存在していれば
        if (effectPrefab != null)
        {
            // エフェクトプレハブを生成
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        // Wall自身は消滅
        Destroy(gameObject);
    }
}
