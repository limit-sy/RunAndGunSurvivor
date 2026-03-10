using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    // 横移動のX軸の限界
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;

    // 体力の最大値
    const int DefaultLife = 3;

    // ダメージをくらった時の硬直時間
    const float StunDuration = 0.5f;

    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;   // 移動すべき量
    int targetLane; // 向かうべきX座標
    int life = DefaultLife; // 現体力
    float recoverTime = 0.0f;   // 復帰までのカウントダウン

    float currentMoveInputX;    // InputSystemの入力値を格納予定
    // Inputを連続で認知しないためのインターバルのコルーチン
    Coroutine resetIntervalCol;

    public float gravity = 20.0f;   // 重力加速度
    public float speedZ = 5.0f; // 前進スピード
    public float speedX = 3.0f; // 横移動スピード
    public float speedJump = 8.0f;  // ジャンプ力
    public float accelerationZ = 10.0f; // 前進加速力

    [Header("ソードのスクリプト")]
    public NormalSword normalSword;

    void OnMove(InputValue value)
    {
        // NormalSwordスクリプトのisSword変数を見て攻撃中なら何もできない
        if (normalSword.GetIsSword()) return;

        // すでに前に入力検知してインターバル中であれば何もしない
        if (resetIntervalCol == null)
        {
            // 検知した値(value)をVector2で表現して変数inputVectorに格納
            Vector2 inputVector = value.Get<Vector2>();
            // 変数inputVectorのうち、X座標にまつわる値を変数currentMoveInputXに格納
            currentMoveInputX = inputVector.x;
        }
    }

    void OnJump(InputValue value)
    {
        // NormalSwordスクリプトのisSword変数を見て攻撃中なら何もできない
        if (normalSword.GetIsSword()) return;

        // ジャンプに関するボタン検知をしたらジャンプメソッド
        Jump();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameState.stageclear || GameManager.gameState == GameState.result) return;
        // InputManagerシステム採用の場合
        //if (Input.GetKeyDown("left")) MoveToLeft();
        //if (Input.GetKeyDown("right")) MoveToRight();
        //if (Input.GetKeyDown("space")) Jump();

        // 左が押されていたら
        if (currentMoveInputX < 0.0f)
        {
            MoveToLeft();
        }
        // 右が押されていたら
        if (currentMoveInputX > 0.0f)
        {
            MoveToRight();
        }

        // 硬直フラグをチェック
        if (IsStun())
        {
            // moveDirectionのxを0
            moveDirection.x = 0;
            // moveDirectionのzを0
            moveDirection.z = 0;
            // recoverTimeをカウントダウン
            recoverTime -= Time.deltaTime;
        }
        else
        {
            // 前進のアルゴリズム
            // その時のmoveDirection.zにaccelerationZの加速度を足していく
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            // 導き出した値に上限を設けてそれをmoveDirection.zとする
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            // 横移動のアルゴリズム
            // 目的地と自分の位置の差を取り、1レーン当たりの幅に対して割合を見る
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            // 割合に変数speedXを係数としてかけた値がmoveDirection.x
            moveDirection.x = ratioX * speedX;
        }

        // 重力の加速度をmoveDirection.y
        moveDirection.y -= gravity * Time.deltaTime;

        // 回転時、自分にとってのZ軸をグローバル座標の値に変換
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        // CharacterControllerコンポーネントのMoveメソッドに授けてPlayerを動かす
        controller.Move(globalDirection * Time.deltaTime);

        // 地面についていたら重力をリセット
        if (controller.isGrounded) moveDirection.y = 0;
    }

    public void MoveToLeft()
    {
        // 硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        // 地面にいる かつ targetがまだ最小でない
        if (controller.isGrounded && targetLane > MinLane)
        {
            targetLane--;
            currentMoveInputX = 0;  // 何も入力していない状況にリセット
            // 次の入力検知を有効にするまでのインターバル
            resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    public void MoveToRight()
    {
        // 硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        // 地面にいる かつ targetがまだ最大でない
        if (controller.isGrounded && targetLane < MaxLane)
        {
            targetLane++;
            currentMoveInputX = 0;  // 何も入力していない状況にリセット
            // 次の入力検知を有効にするまでのインターバル
            resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    IEnumerator ResetIntervalCol()
    {
        // とりあえず0.1秒待つ
        yield return new WaitForSeconds(0.1f);
        resetIntervalCol = null;    // コルーチン情報を解除
    }

    public void Jump()
    {
        // 硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        // 地面にいたら
        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;
        }
    }

    // CharacterControllerコンポーネントが何かとぶつかったとき
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;

        // 相手がEnemyなら
        if (hit.gameObject.tag == "Enemy")
        {
            LifeDown(); // 体力が減る
            GetComponent<NormalShooter>().ShootPowerDown(); // 銃の威力を減らすメソッド
            recoverTime = StunDuration; // 定数の値にrecoverTimeがセッティング

            // 体力がなくなったらゲームオーバー
            if (life <= 0) GameManager.gameState = GameState.gameover;

            //Destroy(hit.gameObject);    // 相手を消滅
            hit.gameObject.GetComponent<Wall>().CreateEffect();
        }
    }

    // ゴールに触れたらステータスをゲームクリアに変更
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            GameManager.gameState = GameState.stageclear;
        }
    }

    // 現在の体力を返す
    public int Life()
    {
        return life;
    }

    // 体力を1回復 (DefaultLife)でバリデーション
    public void LifeUP()
    {
        if (life++ < DefaultLife)
        {
            life++;
        }
        else
        {
            life = DefaultLife;
        }
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateLife(Life());
    }

    // 体力のダメージによる減少
    public void LifeDown()
    {
        life--;
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateLife(Life());
    }

    // Playerを硬直させるべきかチェックするメソッド
    private bool IsStun()
    {
        return (recoverTime > 0.0f) || (life <= 0);
    }
}
