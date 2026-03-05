using System.Collections;
using System.Collections.Generic;

using UnityEditor.Analytics;
using UnityEngine;

public class NejikoController : MonoBehaviour
{
    CharacterController controller;
    //Animator animator;

    Vector3 moveDirection = Vector3.zero;   // 移動するべき量 Vector3.zero→Vector3(0, 0, 0)と同義

    public float gravity = 20;  // 重力加速度
    public float speedZ = 5;    // 前進する力
    public float speedJump = 8; // ジャンプ力

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 必要なコンポーネントを自動取得
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // CharacterControllerコンポーネントが持っている設置のチェック (bool)
        if (controller.isGrounded)
        {
            // 垂直方向のボタン入力をチェック (Vertical ↑ ↓ WS)
            if (Input.GetAxis("Vertical") > 0.0f)
            {
                // このフレームにおける前進/後退の移動量が決まる
                moveDirection.z = Input.GetAxis("Vertical") + speedZ;
            }
            else
            {
                moveDirection.z = 0;
            }

            // 左右キーを押したときの回転
            transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);

            // スペースキー
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = speedJump;
                //animator SetTrigger("jump");
            }
        }

        // ここまででそのフレームの移動するべき量が決まる(moveDirectionのxとy)
        // 重力分の力を毎フレーム増加
        moveDirection.y -= gravity * Time.deltaTime;

        // 移動実行
        // 引数に与えたVector3値を、そのオブジェクトの向きにあわせてグローバルな値としては何が正しいかに変換
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        // Moveメソッドに与えたVector3値分だけ実際にPlayerが動く
        controller.Move(globalDirection * Time.deltaTime);

        // 移動後接地してたらY方向の速度はリセットする
        if (controller.isGrounded) moveDirection.y = 0;

        // 速度が0以降なら走っているフラグをtrueにする
        //animator.Setbool("run", moveDirection.z > 0.0f);
    }
}
