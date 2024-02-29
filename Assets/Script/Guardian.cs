using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Net.WebRequestMethods;

//Scriptable Object 구조 선언
[CreateAssetMenu(fileName = "GuardianStatus", menuName = "Scriptable Object/GuardianStatus")]
public class GuardianStatus : ScriptableObject
{
    public float AttackCycleTime = 1f;
    public float AttackRadius = 5f;
    public int Damage = 1;
    public int MaxTargetCount = 1;
    public int UpgradeCost = 100;
    public Color Color = Color.white;
}

public class Guardian : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _targetEnemys = new List<GameObject>(); //목표 적 정보를 담을 배열

    public GameObject Projectile;
    public GuardianStatus GuardianStatus; //레벨 정보
    public MeshRenderer GuardianRenderer; //업그레이드에 따라 색을 바꿀 렌더러

    public int Level = 0;

    void Start()
    {
        StartCoroutine(Attack());
        GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; //공격 반경을 GuardianStatus에서 불러와 적용
    }

    #region Attack
    IEnumerator Attack()
    {
        if (_targetEnemys.Count > 0) //목표로 설정된 적이 있다면
        {
            SearchEnemy(); // 적 탐색
            foreach (GameObject target in _targetEnemys) //모든 타겟에 대해 반복
            {
                SetRotationByDirection();

                GameObject projectileInst = Instantiate(Projectile, transform.position, Quaternion.identity); //발사체 생성
                if (projectileInst != null)
                {
                    projectileInst.GetComponent<Projectile>().Damage = GuardianStatus.Damage; //발사체 데미지 초기화
                    projectileInst.GetComponent<Projectile>().Target = target; //발사체 타켓 초기화
                }
            }
        }

        yield return new WaitForSeconds(GuardianStatus.AttackCycleTime); //쿨타임 대기

        StartCoroutine(Attack()); //코루틴 재귀
    }
    private void SetRotationByDirection()
    {
        Vector3 targetPos = _targetEnemys[0].transform.position; //가장 먼저 리스트에 들어간 목표의 포지션 할당
        targetPos.y = transform.position.y;

        Vector3 dir = targetPos - transform.position; 
        dir.Normalize(); //목표를 바라볼 방향 계산
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up); //회전 값 적용
    }
    void SearchEnemy()
    {
        int count = 0;

        List<GameObject> tempList = new List<GameObject>(); //임시로 사용할 리스트
        foreach (GameObject target in _targetEnemys) //_targetEnemys 안의 모든 요소에 대해 반복
        {
            if (target != null)
            {
                tempList.Add(target);  //임시 리스트에 _targetEnemys의 요소 추가
                count++; //count 증가
            }

            if (count >= GuardianStatus.MaxTargetCount) //count가 최대 타겟 갯수를 넘었을 떄
            {
                break; //forench문 중지
            }
        }

        _targetEnemys = tempList; //_targetEnemys 업데이트
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")) //트리거 중인 Collider의 태그가 Enemy라면
        {
            if (false == _targetEnemys.Contains(other.gameObject)) //_targetEnemys 리스트에 해당 오브젝트가 없을 때
            {
                _targetEnemys.Add(other.gameObject); //해당 오브젝트 추가
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) //트리거 중인 Collider의 태그가 Enemy라면
        {
            if (true == _targetEnemys.Contains(other.gameObject)) //_targetEnemys 리스트에 해당 오브젝트가 있을 때
            {
                _targetEnemys.Remove(other.gameObject); //해당 오브젝트 제거
            }
        }
    }
    #endregion

    #region Upgrade
    public void Upgrade(GuardianStatus status)
    {
        Level += 1; //레벨 변수 업데이트
        GuardianStatus = status; //인수로 받은 데이터로 GuardianStatus 업데이트

        GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; //공격반경 업데이트
        GuardianRenderer.materials[0].color = GuardianStatus.Color; //Renderer 색 업데이트
    }

    #endregion
}