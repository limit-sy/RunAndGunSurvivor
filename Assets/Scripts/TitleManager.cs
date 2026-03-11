using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [Header("スタート時のシーン")]
    public string nextScene;

    bool inputAvail;

    void Start()
    {
        StartCoroutine(InputAvailCol());
    }

    IEnumerator InputAvailCol()
    {
        yield return new WaitForSeconds(1.0f);
        inputAvail = true;
    }
 
    void OnAttack(InputValue value)
    {
        if (inputAvail)
        {
            SceneChange();
        }
    }

    public void SceneChange()
    {
        //トータルスコアをリセット
        ScoreManager.totalScore = 0;
        SceneManager.LoadScene(nextScene);
    }
}
