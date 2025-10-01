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


    // ��ʼ����Ӱ
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
        // �����������ڽ���
        _timer += Time.deltaTime;
        float progress = _timer / _lifeTime;

        //// �𽥽���͸����
        //Color color = _spriteRenderer.color;
        //color.a = Mathf.Lerp(color.a, 0, progress);
        //_spriteRenderer.color = color;

        //// �����������Ч����ǿ�Ӿ�Ч��
        //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, progress * 0.5f);


        //// �𽥽���͸����
        Color color = _spriteRenderer.color;
        color.a = Mathf.Lerp(initcolora, 0, progress);
        _spriteRenderer.color = color;

        //// �����������Ч����ǿ�Ӿ�Ч��
        transform.localScale = Vector3.Lerp(initlocalScale, Vector3.zero, progress );

        // �������ڽ��������ٲ�Ӱ
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
