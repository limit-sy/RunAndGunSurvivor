using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NormalShooter : MonoBehaviour
{
    [Header("Bullet管理スクリプトと連携")]
    public BulletManager bulletManager;

    [Header("生成オブジェクトと位置")]
    public GameObject bulletPrefabs;//生成対象プレハブ
    public GameObject gate; //生成位置

    [Header("弾速")]
    public float shootSpeed = 10.0f; //弾速

    GameObject bullets; //生成した弾をまとめるオブジェクト
    
    //InputAction(Playerマップ)のAttackアクションがおされたら
    void OnAttack(InputValue value)
    {
        Shoot();
    }

    void Shoot()
    {
       if (bulletManager.GetBulletRemaining() > 0)
        {
            // プレハブの生成と生成情報の取得
            GameObject obj = Instantiate(
                bulletPrefabs,  // 何を
                gate.transform.position,    // どこに
                Quaternion.Euler(90, 0, 0)  // どの角度で
                );

            // 生成したBulletをBulletsオブジェクトのの子供にしてまとめる
            obj.transform.parent = bullets.transform;

            // bulletを消費
            bulletManager.ConsumeBullet();

            // 生成したbullet自身のRigidbodyの力で飛ばす
            Rigidbody bulletRbody = obj.GetComponent<Rigidbody>();
            bulletRbody.AddForce(new Vector3(0, 0, shootSpeed), ForceMode.Impulse);
        }
       else
        {
            // 残数が無ければマガジンを消費して補充開始
            bulletManager.RecoverBullet();
        }
    }

    void Start()
    {
        // 指定したタグを持っているオブジェクトを取得
        bullets = GameObject.FindGameObjectWithTag("Bullets");
    }    
}
