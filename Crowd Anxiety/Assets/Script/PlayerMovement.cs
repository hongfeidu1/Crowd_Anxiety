using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//设计一个点击移动代码，刚体、方向、速度、移动开关，Awake函数绑定刚体，Updare每帧计算数据，FixedUpdate实现匀速移动
//分鼠标与触屏两种逻辑，做两个函数，判定点击状态，屏幕坐标转世界坐标，计算移动方向，进行移动，停止移动

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //鼠标点击
        HandleMouseInput();
        
        //手机触屏
        HandleTouchInput();
    }

    private void FixedUpdate()
    {
        if (!isMoving)//当移动状态为false时，不移动
            return;

        //移动持续匀速
        rb.MovePosition(rb.position + Time.fixedDeltaTime * moveSpeed * moveDirection);
    }

    private void HandleMouseInput()//鼠标
    {
        if (Input.GetMouseButtonDown(0))//检测鼠标是否左键（0）点击，只触发一次（按下瞬间）
        {
            //将点击坐标转换为游戏世界坐标
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //开始移动前的准备：计算方向与更新状态
            StartMove(worldPosition);
        }

        //停止移动
        if (Input.GetMouseButtonUp(0))
        {
            StopMove();
        }

    }

    private void HandleTouchInput()//触屏
    {
        if (Input.touchCount > 0)//检测触摸数量来判定是否触屏
        {
            Touch touch = Input.GetTouch(0);//取触摸信息列表第一位触摸信息，避免微小移动误差
            if (touch.phase == TouchPhase.Began)//检测触摸状态是否为刚触摸，也就是同上的只触发一次逻辑
            {
                //将触摸坐标转换为游戏世界坐标
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                //开始移动前的准备：计算方向与更新状态
                StartMove(worldPosition);
            }

            //停止移动
            if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                StopMove();
            }
        }
    }

    private void StartMove(Vector2 targetPosition)
    {
        //计算方向
        moveDirection = (targetPosition - rb.position).normalized;
        //更新移动状态
        isMoving = true;
    }

    private void StopMove()
    {
        //更新移动状态
        isMoving = false;
    }
}
