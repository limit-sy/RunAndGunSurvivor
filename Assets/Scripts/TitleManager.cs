using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("スタート時のシーン")]
    public string nextScene;
 
    void OnAttack(InputValue value)
    {
        SceneChange();
    }

    public void SceneChange()
    {
        //トータルスコアをリセット
        ScoreManager.totalScore = 0;
        SceneManager.LoadScene(nextScene);
    }
}
