using UnityEngine;

public class EffectController : MonoBehaviour
{
    [Header("削除までの時間")]
    public float deleteTime = 2.0f;

    void Start()
    {
        //生成と同時に時間差で消える
        Destroy(gameObject, deleteTime);
    }
}
