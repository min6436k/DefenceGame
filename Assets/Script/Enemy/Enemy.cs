using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _currentWayPoint; //목표 포인트
    private int _wayPointCount = 0; //지금까지 지난 포인트의 수 (WayPoints의 인덱스)
    private Vector3 _moveDirection = Vector3.zero;
    private int _hp = 5;

    [HideInInspector]
    public GameObject[] WayPoints;
    public int MaxHp = 5;
    public float MoveSpeed = 10;
    public int StealCoin = 100; //드랍할 코인의 양
    public int Damage = 1;

    private void Start()
    {
        _hp = MaxHp; //현재 체력을 최대체력으로 설정
        _currentWayPoint = WayPoints[0]; //현재 지날 포인트를 배열의 첫번째 요소로 설정
        SetRotationByDirection(); //방향전환
    }

    private void Update()
    {
        transform.position += _moveDirection * MoveSpeed * Time.deltaTime; //_moveDirection방향으로 이동

        Vector3 TargetPosition = _currentWayPoint.transform.position; 
        TargetPosition.y = transform.position.y; //목표 포인트와의 거리를 잴 때 필요없는 y값을 똑같이 만듬

        if (Vector3.Distance(transform.position , TargetPosition) <= 0.02f) //목표와의 거ㅏ리가 0.02이하라면
        {
            if(_wayPointCount >= WayPoints.Length - 1) //지난 포인트의 수가 WayPoints의 총 갯수-1 이상일 때
            {
                GameManager.Inst.playerCharacter.Damaged(Damage); //플레이어 체력 감소
                Destroy(gameObject); //Enemy 오브젝트 파괴
                return;
            }

            _wayPointCount = Mathf.Clamp(_wayPointCount + 1, 0, WayPoints.Length); //0과 WayPoints의 갯수의 범위 내에서 _wayPointCount의 값 1 증가
            _currentWayPoint = WayPoints[_wayPointCount]; //목표 포인트를 다음 포인트로 변경

            SetRotationByDirection(); //방향전환
        }
    }

    private void SetRotationByDirection()
    {
        _moveDirection = _currentWayPoint.transform.position - transform.position; //목표 포인트와 오브젝트 간의 위치 차이
        _moveDirection.y = 0; //y값 제거
        _moveDirection.Normalize(); //방향벡터화

        transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up); //y축을 기준으로 해당 방향을 바라보도록 변경
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) //발사체에 닿았을 시
        {
            _hp = Mathf.Clamp(_hp - 1, 0, MaxHp); //0과 최대 체력의 범위 내에서 hp 1 감소

            if (_hp <= 0) //체력이 0 이하로 떨어졌다면
            {
                gameObject.SetActive(false); //오브젝트 비활성화
                GameManager.Inst.EnemyDead(StealCoin); //코인 획득 처리
                Destroy(gameObject); //오브젝트 파괴
            }
        }
    }
}
