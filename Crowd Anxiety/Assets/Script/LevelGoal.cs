using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//通关点，物体上添加组件Collider2D,勾选is Trigger
public class LevelGoal : MonoBehaviour
{
    [Header("LevelController")]
    [Tooltip("GameObject")]
    public LevelController levelController;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        if (levelController != null)
            levelController.ClearLevel();
    }
}
