using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    // 横移動のX軸の限界
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 2.0f;

    // 体力の最大値
    const int DefaultLife = 3;

    // ダメージをくらった時の硬直時間
    const float StunDuration = 0.5f;

    CharacterController controller;
    Animator animator;
    public GameObject animeBody;    // アニメーターを持っている本体
    bool isAnime;   // リトライ・リザルトのアクションを発動させたか

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

    [Header("連続移動の間隔（秒）")]
    public float moveInterval = 0.25f; // 0.25秒ごとに次のレーンへ（お好みで調整）
    //bool canMove = true;              // 移動可能フラグ

    [Header("スティック長押しリピートの設定")]
    public float initialDelay = 0.4f; // 最初の1回目から2回目までの待ち時間
    public float repeatInterval = 0.15f; // 2回目以降の連続移動の間隔
    private float nextMoveTimer = 0f; // 次の移動を許可するまでのタイマー
    private bool isContinuousMoving = false; // 連続移動モードに入っているか

    [Header("ソードのスクリプト")]
    public NormalSword normalSword;

    AudioSource[] playerAudio;
    //足音判定
    float footstepInterval = 0.3f; // 足音間隔
    float footstepTimer; // 時間計測
    [Header("SE音源")]
    public AudioClip se_Walk;
    public AudioClip se_Damage;
    public AudioClip se_Explosion;
    public AudioClip se_Jump;
    public AudioClip se_Dash;
    public AudioClip se_Reload;

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
        animator = animeBody.GetComponent<Animator>();
        playerAudio = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameState.stageclear || GameManager.gameState == GameState.result) return;
        // InputManagerシステム採用の場合
        //if (Input.GetKeyDown("left")) MoveToLeft();
        //if (Input.GetKeyDown("right")) MoveToRight();
        //if (Input.GetKeyDown("space")) Jump();

        // --- 連続移動リピートのアルゴリズム ---
        if (!IsStun() && Mathf.Abs(currentMoveInputX) > 0.5f) // スティックが倒されている
        {
            if (Time.time >= nextMoveTimer)
            {
                // 左が押されていたら
                if (currentMoveInputX < -0.5f)
                {
                    MoveToLeft();
                }
                // 右が押されていたら
                if (currentMoveInputX > 0.5f)
                {

                    MoveToRight();
                }

                // 次のタイマーを設定
                if (!isContinuousMoving)
                {
                    // 初回の移動直後：長めの待ち時間を設定
                    nextMoveTimer = Time.time + initialDelay;
                    isContinuousMoving = true;
                }
                else
                {
                    // 2回目以降の移動直後：短い間隔を設定
                    nextMoveTimer = Time.time + repeatInterval;
                }
            }
        }
        else
        {
            // スティックを離したらリセット
            isContinuousMoving = false;
            nextMoveTimer = 0f;
        }

        //if (canMove && !IsStun())
        //{
            // 左が押されていたら
        //    if (currentMoveInputX < -0.5f)
        //    {
        //        MoveToLeft();
        //    }
            // 右が押されていたら
        //    if (currentMoveInputX > 0.5f)
        //    {
        //
        //        MoveToRight();
        //    }
        //}

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

        // 足音メソッド
        HandleFootsteps();
    }

    // 足音メソッド
    void HandleFootsteps()
    {
        // 地面にいてプレイヤーが動いていれば
        if (controller.isGrounded && moveDirection.z != 0)
        {
            footstepTimer += Time.deltaTime; // 時間計測

            if (footstepTimer >= footstepInterval) // インターバルチェック
            {
                playerAudio[1].PlayOneShot(se_Walk);
                footstepTimer = 0;
            }
        }
        else // 動いていなければ時間計測リセット
        {
            footstepTimer = 0f;
        }
    }

    public void MoveToLeft()
    {
        // 硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        // 地面にいる かつ targetがまだ最小でない
        if (controller.isGrounded && targetLane > MinLane)
        {
            playerAudio[0].PlayOneShot(se_Dash);
            targetLane--;

            //currentMoveInputX = 0;  // 何も入力していない状況にリセット
            //// 次の入力検知を有効にするまでのインターバル
            //resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    public void MoveToRight()
    {
        // 硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        // 地面にいる かつ targetがまだ最大でない
        if (controller.isGrounded && targetLane < MaxLane)
        {
            playerAudio[0].PlayOneShot(se_Dash);
            targetLane++;

            //currentMoveInputX = 0;  // 何も入力していない状況にリセット
            //// 次の入力検知を有効にするまでのインターバル
            //resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    IEnumerator ResetMoveInterval()
    {
        yield return new WaitForSeconds(moveInterval);
        //canMove = true;
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
            animator.SetTrigger("jump");
            playerAudio[0].PlayOneShot(se_Jump);
        }
    }

    // CharacterControllerコンポーネントが何かとぶつかったとき
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;

        // 相手がEnemyなら
        if (hit.gameObject.tag == "Enemy")
        {
            playerAudio[2].PlayOneShot(se_Damage);
            LifeDown(); // 体力が減る
            GetComponent<NormalShooter>().ShootPowerDown(); // 銃の威力を減らすメソッド
            recoverTime = StunDuration; // 定数の値にrecoverTimeがセッティング

            // 体力がなくなったらゲームオーバー
            if (life <= 0)
            {
                GameManager.gameState = GameState.gameover;
                if (!isAnime)
                {
                    animator.SetTrigger("retry");
                    isAnime = true;
                }
            }
            animator.SetTrigger("damage");
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
            if (!isAnime)
            {
                animator.SetTrigger("result");
                isAnime = true;
                playerAudio[0].PlayOneShot(se_Reload);
            }
            Destroy(other.gameObject);  // ゴールしたらゴールオブジェクトを抹消
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
