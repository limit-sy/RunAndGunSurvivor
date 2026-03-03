using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float deleteTime = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,deleteTime);
        
    }

    private void OnTriggerEnter(Collider other)
    {
            Destroy(gameObject);
    }
}
