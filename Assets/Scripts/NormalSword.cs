using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NormalSword : MonoBehaviour
{
    [Header("対象ソードオブジェクト")]
    public GameObject swordObject;

    [Header("ソード威力・発生時間（硬直時間）")]
    public int swordPower = 3;
    public float swordTime = 0.5f;

    Coroutine swordAttackCol; //発生中コルーチン
    bool isSword; //外部に参照させるソード発生中フラグ

    void OnCrouch(InputValue value)
    {
        SwordAttack();
    }
    void Start()
    {
        //初期状態では存在しない
        swordObject.SetActive(false);
    }

    //存在させる
    void SwordAttack()
    {
        if (swordAttackCol == null)
        {
            //コルーチン発動によりソードを存在させる
            swordAttackCol = StartCoroutine(SwordAttackCol());
        }
    }

    //ソード発生コルーチン
    IEnumerator SwordAttackCol()
    {
        swordObject.SetActive(true); //存在
        isSword = true; //ソード発生フラグ
        yield return new WaitForSeconds(swordTime); //消すまでのインターバル

        swordObject.SetActive(false); //存在を消す
        isSword = false; //ソード発生フラグをOFF
        swordAttackCol = null; //コルーチンを解放
    }

    //ソード威力を取得
    public int GetSwordPower()
    {
        return swordPower;
    }

    //ソード発生フラグの取得
    public bool GetIsSword()
    {
        return isSword;
    }
}
