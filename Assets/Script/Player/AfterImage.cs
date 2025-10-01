using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private float _lifeTime;
    private float _timer;
    private float initcolora;
    private Vector3 initlocalScale;


    // 初始化残影
    public void Initialize(float lifeTime)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lifeTime = lifeTime;
        _timer = 0;
        initcolora=_spriteRenderer.material.color.a;
        initlocalScale = transform.localScale;
    }

    private void Update()
    {
        // 计算生命周期进度
        _timer += Time.deltaTime;
        float progress = _timer / _lifeTime;

        //// 逐渐降低透明度
        //Color color = _spriteRenderer.color;
        //color.a = Mathf.Lerp(color.a, 0, progress);
        //_spriteRenderer.color = color;

        //// 可以添加缩放效果增强视觉效果
        //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, progress * 0.5f);


        //// 逐渐降低透明度
        Color color = _spriteRenderer.color;
        color.a = Mathf.Lerp(initcolora, 0, progress);
        _spriteRenderer.color = color;

        //// 可以添加缩放效果增强视觉效果
        transform.localScale = Vector3.Lerp(initlocalScale, Vector3.zero, progress );

        // 生命周期结束后销毁残影
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
