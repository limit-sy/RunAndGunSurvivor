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
       
    }

    void Start()
    {
        
    }    
}
