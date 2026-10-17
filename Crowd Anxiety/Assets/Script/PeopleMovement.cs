using UnityEngine;

//与其说是状态机，其实只是行为选择器，因为没有切换状态

[RequireComponent(typeof(Rigidbody2D))]//写在类定义上方，用来给没有组件的对象添加对应组件
public class PeopleMovement : MonoBehaviour
{
    public enum MovementType
    {
        Idle,       //静止不动
        Wander,     //小范围随机移动
        Straight,   //单方向持续移动
        Patrol,     //固定线段往返
        Random      //开局随机一个以上状态，回头改
    }

    [Header("运动状态")]
    public MovementType movementType = MovementType.Random;

    [Header("通用速度")]
    public float moveSpeed = 1f;

    [Header("Wander范围")]
    [Tooltip("半径")]
    public float wanderRadius = 2f;
    [Tooltip("到目标点的停留时间")]
    public float wanderPauseMin = 0.3f;
    public float wanderPauseMax = 1.5f;
    [Tooltip("找一个目标点的花费上限，时间")]
    public float wanderTimeout = 5f;

    [Header("Straight移动方向")]
    public Vector2 straightDirection = Vector2.right;

    [Header("Patrol两个端点相对出生位置的偏移")]
    public Vector2 patrolOffsetA = new Vector2(-2f, 0f);
    public Vector2 patrolOffsetB = new Vector2(2f, 0f);
    [Tooltip("端点停留时间")]
    public float patrolPause = 0.5f;

    [Header("调试")]
    [SerializeField] private MovementType currentType;

    private Rigidbody2D rb;
    private Vector2 home;//出生点
    private Vector2 moveTarget;
    private float pauseTimer;//停留时间
    private float wanderTimer;//记录找寻目的地时间，用来判定是否结束本次找寻
    private int patrolIndex;//0-A, 1-B

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        home = rb.position;
        //随机一个运动状态，后面改
        currentType = movementType == MovementType.Random? (MovementType)Random.Range(0,4) : movementType;

        if (currentType == MovementType.Wander)
            PickWanderTarget();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 velocity = Vector2.zero;

        switch (currentType)
        {
            case MovementType.Idle:
                velocity = Vector2.zero;
                break;

            case MovementType.Wander:
                velocity = UpdateWander();
                break;

            case MovementType.Straight:
                velocity = straightDirection.normalized * moveSpeed;
                break;

            case MovementType.Patrol:
                velocity = UpdatePatrol();
                break;
        }

        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private Vector2 UpdatePatrol()
    {
        if(pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return Vector2.zero;
        }

        Vector2 point = home + (patrolIndex == 0? patrolOffsetA : patrolOffsetB);
        Vector2 toPoint = point - rb.position;

        if(toPoint.magnitude < 0.05f)
        {
            pauseTimer = patrolPause;
            patrolIndex = 1 - patrolIndex;
            return Vector2.zero;
        }

        return toPoint.normalized * moveSpeed;
    }

    private Vector2 UpdateWander()
    {
        if(pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return Vector2.zero;
        }

        wanderTimer += Time.fixedDeltaTime;
        if(wanderTimer > wanderTimeout)
        {
            PickWanderTarget();//再找点
            return Vector2.zero;
        }

        Vector2 toTarget = moveTarget - rb.position;//目的地向量
        if(toTarget.magnitude < 0.05f)//判定模长
        {
            pauseTimer = Random.Range(wanderPauseMin, wanderPauseMax);
            PickWanderTarget();//再找点
            return Vector2.zero;
        }

        return toTarget.normalized * moveSpeed;
    }

    private void PickWanderTarget()
    {
        //出生点画圆，随机取点
        moveTarget = home + Random.insideUnitCircle * wanderRadius;//Random.insideUnitCircle向量，半径为1圆形内部
        wanderTimer = 0f;
    }
}
