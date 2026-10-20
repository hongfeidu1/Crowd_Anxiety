using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//设计一个点击移动代码，刚体、方向、速度、移动开关，Awake函数绑定刚体，Updare每帧计算数据，FixedUpdate实现匀速移动
//分鼠标与触屏两种逻辑，做两个函数，判定点击状态，屏幕坐标转世界坐标，计算移动方向，进行移动，停止移动

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("被撞后惯性")]
    [Tooltip("每秒保留的速度比例")]
    [Range(0f, 1f)] public float idleVelocityRetain = 0.1f;
    [Tooltip("低于该值速度归零，避免计算始终无法归零")]
    [SerializeField] private float stopSpeedThreshold = 0.05f;

    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private bool isMoving = false;
    private int activeFingerId = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        isMoving = false;
        activeFingerId = -1;
    }


    private void Update()
    {
        //理论上可以直接合并，不过为了练手还是保留了
        if(Input.touchCount > 0)
        {
            //手机触屏
            HandleTouchInput();
        }
        else
            //鼠标点击
            HandleMouseInput();
    }

    private void FixedUpdate()
    {
        if (!isMoving)//当移动状态为false时，不移动
        {
            //因为碰撞挤压，会有惯性移动
            //衰减
            //通过Pow(x, y)计算x的y次方，求单帧保留系数，每帧执行后实现每秒保留速度比例的衰减
            Vector2 v = rb.velocity * Mathf.Pow(idleVelocityRetain, Time.fixedDeltaTime);

            rb.velocity = v.sqrMagnitude < stopSpeedThreshold * stopSpeedThreshold
                ? Vector2.zero : v;
            return;
        }

        //移动持续匀速
        rb.velocity = moveDirection * moveSpeed;
    }

    private void HandleMouseInput()//鼠标
    {
        //检测鼠标是否左键（0）点击，只触发一次（按下瞬间）
        //检测是否点在UI上了，虽然我没做设置或其它什么关卡内部UI
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI(-1))
        {
            StartMove(ScreenToWorld(Input.mousePosition));
        }
        else if (Input.GetMouseButton(0) && isMoving)
            UpdateDirection(ScreenToWorld(Input.mousePosition));

        //停止移动
        if (Input.GetMouseButtonUp(0))
        {
            StopMove();
        }

    }

    private void HandleTouchInput()//触屏
    {
        //循环遍历本次按下记录的所有点
        for(int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (activeFingerId == -1)
            {
                if (touch.phase != TouchPhase.Began) continue;
                if (IsPointerOverUI(touch.fingerId)) continue;

                activeFingerId = touch.fingerId;
                StartMove(ScreenToWorld(touch.position));
            }

            if (touch.fingerId != activeFingerId) continue;

            switch (touch.phase)
            {
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    UpdateDirection(ScreenToWorld(touch.position));
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    activeFingerId = -1;
                    StopMove();
                    break;
            }
        }
    }

    private Vector2 ScreenToWorld(Vector3 mousePosition)
    {
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    //是否点在UI上了
    //current是EventSystem的静态属性
    private bool IsPointerOverUI(int fingerId)
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private void StartMove(Vector2 targetPosition)
    {
        //计算方向
        UpdateDirection(targetPosition);
        //moveDirection = (targetPosition - rb.position).normalized;
        //更新移动状态
        isMoving = true;
    }

    //计算方向
    private void UpdateDirection(Vector2 targetPosition)
    {
        Vector2 delta = targetPosition - rb.position;
        if (delta.sqrMagnitude > 0.0001f)
            moveDirection = delta.normalized;
    }

    private void StopMove()
    {
        //更新移动状态
        isMoving = false;
        moveDirection = Vector2.zero;//一个脏数据
        rb.velocity = Vector2.zero;
    }
}
