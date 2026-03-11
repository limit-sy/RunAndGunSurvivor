using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource soundAudio;
    [Header("ステージ曲")]
    public AudioClip battleBGM;
    public AudioClip retryBGM;
    public AudioClip resultBGM;
    bool isChangeBGM; //切り替え済みか

    void Start()
    {
        soundAudio = GetComponent<AudioSource>();
        StartCoroutine(BattleBGMStartCol());
    }

    IEnumerator BattleBGMStartCol()
    {
        soundAudio.Stop();
        soundAudio.clip = battleBGM;
        yield return new WaitForSeconds(1.0f);
        soundAudio.Play();
    }

    void Update()
    {
        if (!isChangeBGM)
        {
            if (GameManager.gameState == GameState.result)
            {
                soundAudio.Stop();
                soundAudio.clip = resultBGM;
                soundAudio.Play();
                isChangeBGM = true;
            }
            else if (GameManager.gameState == GameState.retry)
            {
                soundAudio.Stop();
                soundAudio.clip = retryBGM;
                soundAudio.Play();
                isChangeBGM = true;
            }
        }
    }
}
