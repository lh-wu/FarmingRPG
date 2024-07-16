using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实现碰撞摇晃效果
/// </summary>
public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x < collision.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x > collision.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }

    private IEnumerator RotateClock()
    {
        isAnimating = true;

        //逆时针旋转4个单位
        for(int i = 0; i < 4; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }

        //顺时针旋转5个单位
        for (int i = 0; i < 5; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }

        //回正(5-4)=1
        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
        yield return pause;

        isAnimating = false;
                    
    }

    private IEnumerator RotateAntiClock()
    {
        isAnimating = true;

        //顺时针旋转4个单位
        for (int i = 0; i < 4; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }

        //逆时针旋转5个单位
        for (int i = 0; i < 5; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }

        //回正(5-4)=1
        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
        yield return pause;

        isAnimating = false;
    }
}
