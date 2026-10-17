using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一股人流：固定矩形区域内生成count个人，向着一个方向移动
//走出边界后传送回起点，形成不间断的循环人流
//不用对象池
public class PeopleStream : MonoBehaviour
{
    [Header("预制体")]
    public GameObject peoplePrefab;

    [Header("移动方向")]
    public Vector2 direction = Vector2.right;

    [Header("人潮范围")]
    [Tooltip("长")]
    public float length = 30f;
    [Tooltip("宽")]
    public float width = 4f;

    [Header("人数")]
    public int count = 20;

    [Header("速度")]
    [Tooltip("基础速度")]
    public float speed = 3f;
    [Tooltip("浮动比例，0.2=每人正负20%速度差异")]
    [Range(0f, 1f)] public float speedVariation = 0.2f;

    [Header("回收")]
    [Tooltip("走出多远才传送回去")]
    public float despawnMargin = 2f;

    private Transform[] members;
    private Rigidbody2D[] bodies;

    private Vector2 dir;//归一后的方向
    private Vector2 perp;//垂直的方向
    private Vector2 center;//区域中心位置

    private void Start()
    {
        if (peoplePrefab == null) return;

        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        perp = new Vector2(-dir.y, dir.x);
        center = transform.position;

        var memberList = new List<Transform>(count);
        var bodyList = new List<Rigidbody2D>(count);

        for(int i = 0; i < count; i++)
        {
            //将列表种的对象沿着移动方向随机排布，垂直移动方向随机排布
            float along = Random.Range(-length * 0.5f, length * 0.5f);
            float across = Random.Range(-width * 0.5f, width * 0.5f);
            //坐标计算，中心坐标 + 移动方向 * 长度范围 + 垂直方向 * 宽度范围 
            Vector2 pos = center + dir * along + perp * across;

            //实例化Instantiate
            //Quaternion.identity旋转，代表无旋转（0，0，0）
            GameObject go = Instantiate(peoplePrefab, pos, Quaternion.identity, transform);

            var move = go.GetComponent<PeopleMovement>();
            if (move != null)
            {
                //要保证覆盖PeopleMovement的移动状态
                move.movementType = PeopleMovement.MovementType.Straight;
                move.straightDirection = dir;
                move.moveSpeed = speed * Random.Range(1f - speedVariation, 1f + speedVariation);
            }

            memberList.Add(go.transform);
            bodyList.Add(go.GetComponent<Rigidbody2D>());
        }

        members = memberList.ToArray();
        bodies = bodyList.ToArray();
    }

    private void FixedUpdate()
    {
        if(members == null) return;
        //计算传送位置
        float exitLine = length * 0.5f + despawnMargin;
        float enterLine = -(length * 0.5f + despawnMargin);

        for (int i = 0; i < members.Length; i++)
        {
            Vector2 pos = bodies[i] != null ? bodies[i].position : (Vector2)members[i].position;
            //走了多远，点积算投影向量
            float along = Vector2.Dot(pos - center, dir);

            if (along > exitLine)
            {
                //传送到人流尾部，宽位置随机
                float across = Random.Range(-width * 0.5f, width * 0.5f);
                Vector2 newPos = center + dir * enterLine + perp * across;

                if (bodies[i] != null)
                    bodies[i].position = newPos;
                else
                    members[i].position = newPos;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 d = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Vector2 p = new Vector2(-d.y, d.x);

        Vector3 a = transform.position + (Vector3)(d * (length * 0.5f) + p * (width * 0.5f));
        Vector3 b = transform.position + (Vector3)(d * (length * 0.5f) - p * (width * 0.5f));
        Vector3 c = transform.position + (Vector3)(d * (-length * 0.5f) - p * (width * 0.5f));
        Vector3 e = transform.position + (Vector3)(d * (-length * 0.5f) + p * (width * 0.5f));

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, e);
        Gizmos.DrawLine(e, a);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(d * 2f));
    }
}
