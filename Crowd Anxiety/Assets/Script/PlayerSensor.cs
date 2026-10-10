using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public float detectionRadius = 5f;

    public struct PersonInfo
    {
        public Transform transform;
        public float distance;
    }

    private readonly List<PersonInfo> peopleBuffer = new List<PersonInfo>();
    public IReadOnlyList<PersonInfo> People => peopleBuffer;//外部只读

    //——————这波傻逼了，竟然用两个列表存
    //列表存储检测到的人
    //public List<GameObject> peopleList = new();
    //列表存储每个范围内人的距离
    //public List<float> distanceList = new();

    //每帧检测
    private void Update()
    {
        DetectedPeople();
    }

    //检测人
    private void DetectedPeople()
    {
        //初始化清空列表,每帧进行
        //peopleList.Clear();
        //distanceList.Clear();
        peopleBuffer.Clear();

        //做一个圆形collider2D范围，以自身transform.position为圆心，半径detectionRadius做圆
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        //获取所有范围内的collider
        foreach (Collider2D collider in colliders)//foreach专用来遍历集合/数组
        {
            //判断tag
            if (collider.CompareTag("People"))
            {
                //GameObject people = collider.gameObject;

                //peopleList.Add(people);//人加入列表
                peopleBuffer.Add(new PersonInfo
                {
                    transform = collider.transform,
                    distance = Vector2.Distance(transform.position, collider.transform.position)
                });

                //float distance = Vector2.Distance(transform.position, people.transform.position);//计算距离

                //distanceList.Add(distance);//距离加入列表
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //临时用范围可视化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
