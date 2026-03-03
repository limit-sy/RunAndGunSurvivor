using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("生成プレハブオブジェクト")]
    public GameObject effectPrefab; //生成プレハブ

    [Header("耐久力")]
    public float life = 5.0f; //耐久力

    [Header("ダメージ時間・振動対象・振動スピード・振動量")]
    public float damegeTime = 0.25f; //ダメージ中時間
    public GameObject damageBody; //振動対象オブジェクト
    public float speed = 75.0f; //振動スピード
    public float amplitude = 1.5f;  //振動量

    Vector3 startPosition; //振動対象の初期位置
    float z; //振動による移動座標

    Coroutine currentDamage; //ダメージコルーチン

    void Start()
    {
       
    }

    void Update()
    {
        
    }

    //衝突
    void OnTriggerEnter(Collider other)
    {
      
    }

    //ダメージコルーチン
    IEnumerator DamageCol()
    {        
        yield return new WaitForSeconds(0.1f);
        
    }
}
