using UnityEngine;

//アイテムの種類の定義
public enum ItemType
{
    Magazine,
    ShootPower,
    LifeUp
}

public class Item : MonoBehaviour
{
    [Header("アイテムの種類")]
    public ItemType type;

    [Header("取得時のエフェクト")]
    public GameObject effectPrefab;

    [Header("生成から削除までの時間")]
    public float deleteTime = 15.0f;    

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {            
            switch (type)
            {
                case ItemType.Magazine:
                    other.gameObject.GetComponent<BulletManager>().AddMagazine();
                    break;
                case ItemType.ShootPower:
                    other.gameObject.GetComponent<NormalShooter>().ShootPowerUp();
                    break;
                case ItemType.LifeUp:
                    other.gameObject.GetComponent<PlayerRun>().LifeUP();
                    break;
            }

            EffectCreate(); //いずれにしてもエフェクト生成
        }
    }

    //エフェクト生成
    void EffectCreate()
    {
        Instantiate(
            effectPrefab,
            transform.position,
            Quaternion.identity
            );

        Destroy(gameObject); //エフェクト発生後はアイテム自身は削除
    }
}