using System.Collections;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    const int maxRemaining = 10; //充填数の上限

    [Header("弾数・保有マガジン数")]
    public int bulletRemaining = maxRemaining; //残弾数
    public int magazine = 1; //マガジン数 ※充填時に消費

    [Header("充填時間")]
    public float recoveryTime = 3.0f; //マガジン補充時間
    float counter; //充填までの残時間

    Coroutine bulletRecover; //発生中のコルーチン情報の参照用

    //弾の消費
    public void ConsumeBullet()
    {
       
    }

    //残数の取得
    public int GetBulletRemaining()
    {
        return 10;
    }

    //弾の充填
    public void AddBullet(int num)
    {
       
    }

    //充填メソッド
    public void RecoverBullet()
    {
       
    }

    //充填コルーチン
    IEnumerator RecoverBulletCol()
    {
       
        yield return new WaitForSeconds(1.0f); //ウェイト処理
         
    }

    //画面上に簡易GUI表示
    void OnGUI()
    {
       
    }
}
