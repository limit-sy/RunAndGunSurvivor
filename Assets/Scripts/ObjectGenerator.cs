using System.Collections;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    //生成レーンの情報
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;

    [Header("ゴール生成される目標座標")]
    public int goalPosition;

    [Header("生成物（エネミー・アイテム・トラップ・ゴール")]
    public GameObject[] enemyObjects;
    public GameObject[] itemObjects;
    public GameObject[] trapObjects;
    public GameObject goalPrefab;

    [Header("残数確認のためのBulletMangerスクリプト")]
    public BulletManager bulletManager;

    bool createGoal; //ゴール生成フラグ

    [Header("最大インターバル(エネミー・アイテム・トラップ)")]
    public float maxEnemyIntervalTime = 3.0f;
    public float maxItemIntervalTime = 8.0f;
    public float maxTrapIntervalTime = 5.0f;

    //それぞれの生成インターバルのためのコルーチン
    Coroutine enemyObjectGenerateCol;
    Coroutine itemObjectGenerateCol;
    Coroutine trapObjectGenerateCol;
    

    void Update()
    {
        //ゴール生成済みでももう何もしない
        if (GameManager.gameState == GameState.gameover || GameManager.gameState == GameState.retry || createGoal) return;

        //目標座標が来たらゴール生成コルーチンとゴール生成フラグをON
        if (transform.position.z > goalPosition)
        {
            StartCoroutine(GoalObjectGenerateCol());
            createGoal = true;
        }
        else //そうでなければエネミー・アイテム・トラップをそれぞれ独立して生成していく
        {
            if (enemyObjectGenerateCol == null)
            {
                enemyObjectGenerateCol = StartCoroutine(EnemyObjectGenerateCol());
            }
            if (itemObjectGenerateCol == null)
            {
                itemObjectGenerateCol = StartCoroutine(ItemObjectGenerateCol());
            }
            if (trapObjectGenerateCol == null)
            {
                trapObjectGenerateCol = StartCoroutine(TrapObjectGenerateCol());
            }
        }

    }

    //エネミー生成コルーチン
    IEnumerator EnemyObjectGenerateCol()
    {
        //ランダムな数字を取得してウェイト
        float generationInterval = Random.Range(1, maxEnemyIntervalTime + 1.0f);
        yield return new WaitForSeconds(generationInterval);

        //ランダムな生成物と生成レーン番号を取得
        int index = Random.Range(0, enemyObjects.Length);
        int targetLane = Random.Range(MinLane, MaxLane + 1);

        Instantiate(
            enemyObjects[index],
            new Vector3(targetLane * LaneWidth, 1, transform.position.z),
            Quaternion.identity
            );
        enemyObjectGenerateCol = null;　//コルーチンの解放

    }

    //トラップ生成コルーチン
    IEnumerator TrapObjectGenerateCol()
    {
        //ランダムな数字を取得してウェイト
        float generationInterval = Random.Range(1, maxTrapIntervalTime + 1.0f);
        yield return new WaitForSeconds(generationInterval);

        //ランダムな生成物を取得
        int index = Random.Range(0, trapObjects.Length);
        Instantiate(
            trapObjects[index],
            new Vector3(0, 1, transform.position.z),
            Quaternion.identity
            );
        trapObjectGenerateCol = null;　//コルーチンの解放
    }

    //アイテム生成コルーチン
    IEnumerator ItemObjectGenerateCol()
    {
        //ランダムな数字を取得してウェイト
        float generationInterval = Random.Range(5, maxItemIntervalTime + 1.0f);
        yield return new WaitForSeconds(generationInterval);

        //残弾・マガジンが0なら優先してmagazine補充
        int index;
        if (bulletManager.GetBulletRemaining() <= 0 && bulletManager.GetMagazineRemaining() <= 0)
        {
            index = 0;
        }
        else //残数があればオブジェクトをランダムに選ぶ
        {
            index = Random.Range(0, itemObjects.Length);
        }
        //ランダムな生成レーン番号を取得
        int targetLane = Random.Range(MinLane, MaxLane + 1);
        Instantiate(
            itemObjects[index],
            new Vector3(targetLane * LaneWidth, 1, transform.position.z),
            Quaternion.identity
            );
        itemObjectGenerateCol = null;　//コルーチンの解放
    }

    //ゴール生成コルーチン
    IEnumerator GoalObjectGenerateCol()
    {
        yield return new WaitForSeconds(15.0f); //他のオブジェクトの生成があらかた終わるまでまつ
        //ゴールの生成
        Instantiate(
            goalPrefab,
            new Vector3(0, 1, transform.position.z),
            Quaternion.identity
            );
    }

}
