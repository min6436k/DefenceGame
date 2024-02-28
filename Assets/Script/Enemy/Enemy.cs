using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _currentWayPoint; //��ǥ ����Ʈ
    private int _wayPointCount = 0; //���ݱ��� ���� ����Ʈ�� �� (WayPoints�� �ε���)
    private Vector3 _moveDirection = Vector3.zero;
    private int _hp = 5;

    [HideInInspector]
    public GameObject[] WayPoints;
    public int MaxHp = 5;
    public float MoveSpeed = 10;
    public int StealCoin = 100; //����� ������ ��
    public int Damage = 1;

    private void Start()
    {
        _hp = MaxHp; //���� ü���� �ִ�ü������ ����
        _currentWayPoint = WayPoints[0]; //���� ���� ����Ʈ�� �迭�� ù��° ��ҷ� ����
        SetRotationByDirection(); //������ȯ
    }

    private void Update()
    {
        transform.position += _moveDirection * MoveSpeed * Time.deltaTime; //_moveDirection�������� �̵�

        Vector3 TargetPosition = _currentWayPoint.transform.position; 
        TargetPosition.y = transform.position.y; //��ǥ ����Ʈ���� �Ÿ��� �� �� �ʿ���� y���� �Ȱ��� ����

        if (Vector3.Distance(transform.position , TargetPosition) <= 0.02f) //��ǥ���� �Ť����� 0.02���϶��
        {
            if(_wayPointCount >= WayPoints.Length - 1) //���� ����Ʈ�� ���� WayPoints�� �� ����-1 �̻��� ��
            {
                GameManager.Inst.playerCharacter.Damaged(Damage); //�÷��̾� ü�� ����
                Destroy(gameObject); //Enemy ������Ʈ �ı�
                return;
            }

            _wayPointCount = Mathf.Clamp(_wayPointCount + 1, 0, WayPoints.Length); //0�� WayPoints�� ������ ���� ������ _wayPointCount�� �� 1 ����
            _currentWayPoint = WayPoints[_wayPointCount]; //��ǥ ����Ʈ�� ���� ����Ʈ�� ����

            SetRotationByDirection(); //������ȯ
        }
    }

    private void SetRotationByDirection()
    {
        _moveDirection = _currentWayPoint.transform.position - transform.position; //��ǥ ����Ʈ�� ������Ʈ ���� ��ġ ����
        _moveDirection.y = 0; //y�� ����
        _moveDirection.Normalize(); //���⺤��ȭ

        transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up); //y���� �������� �ش� ������ �ٶ󺸵��� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) //�߻�ü�� ����� ��
        {
            _hp = Mathf.Clamp(_hp - 1, 0, MaxHp); //0�� �ִ� ü���� ���� ������ hp 1 ����

            if (_hp <= 0) //ü���� 0 ���Ϸ� �������ٸ�
            {
                gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
                GameManager.Inst.EnemyDead(StealCoin); //���� ȹ�� ó��
                Destroy(gameObject); //������Ʈ �ı�
            }
        }
    }
}
