using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Net.WebRequestMethods;

//Scriptable Object ���� ����
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
    private List<GameObject> _targetEnemys = new List<GameObject>(); //��ǥ �� ������ ���� �迭

    public GameObject Projectile;
    public GuardianStatus GuardianStatus; //���� ����
    public MeshRenderer GuardianRenderer; //���׷��̵忡 ���� ���� �ٲ� ������

    public int Level = 0;

    void Start()
    {
        StartCoroutine(Attack());
        GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; //���� �ݰ��� GuardianStatus���� �ҷ��� ����
    }

    #region Attack
    IEnumerator Attack()
    {
        if (_targetEnemys.Count > 0) //��ǥ�� ������ ���� �ִٸ�
        {
            SearchEnemy(); // �� Ž��
            foreach (GameObject target in _targetEnemys) //��� Ÿ�ٿ� ���� �ݺ�
            {
                SetRotationByDirection();

                GameObject projectileInst = Instantiate(Projectile, transform.position, Quaternion.identity); //�߻�ü ����
                if (projectileInst != null)
                {
                    projectileInst.GetComponent<Projectile>().Damage = GuardianStatus.Damage; //�߻�ü ������ �ʱ�ȭ
                    projectileInst.GetComponent<Projectile>().Target = target; //�߻�ü Ÿ�� �ʱ�ȭ
                }
            }
        }

        yield return new WaitForSeconds(GuardianStatus.AttackCycleTime); //��Ÿ�� ���

        StartCoroutine(Attack()); //�ڷ�ƾ ���
    }
    private void SetRotationByDirection()
    {
        Vector3 targetPos = _targetEnemys[0].transform.position; //���� ���� ����Ʈ�� �� ��ǥ�� ������ �Ҵ�
        targetPos.y = transform.position.y;

        Vector3 dir = targetPos - transform.position; 
        dir.Normalize(); //��ǥ�� �ٶ� ���� ���
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up); //ȸ�� �� ����
    }
    void SearchEnemy()
    {
        int count = 0;

        List<GameObject> tempList = new List<GameObject>(); //�ӽ÷� ����� ����Ʈ
        foreach (GameObject target in _targetEnemys) //_targetEnemys ���� ��� ��ҿ� ���� �ݺ�
        {
            if (target != null)
            {
                tempList.Add(target);  //�ӽ� ����Ʈ�� _targetEnemys�� ��� �߰�
                count++; //count ����
            }

            if (count >= GuardianStatus.MaxTargetCount) //count�� �ִ� Ÿ�� ������ �Ѿ��� ��
            {
                break; //forench�� ����
            }
        }

        _targetEnemys = tempList; //_targetEnemys ������Ʈ
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")) //Ʈ���� ���� Collider�� �±װ� Enemy���
        {
            if (false == _targetEnemys.Contains(other.gameObject)) //_targetEnemys ����Ʈ�� �ش� ������Ʈ�� ���� ��
            {
                _targetEnemys.Add(other.gameObject); //�ش� ������Ʈ �߰�
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) //Ʈ���� ���� Collider�� �±װ� Enemy���
        {
            if (true == _targetEnemys.Contains(other.gameObject)) //_targetEnemys ����Ʈ�� �ش� ������Ʈ�� ���� ��
            {
                _targetEnemys.Remove(other.gameObject); //�ش� ������Ʈ ����
            }
        }
    }
    #endregion

    #region Upgrade
    public void Upgrade(GuardianStatus status)
    {
        Level += 1; //���� ���� ������Ʈ
        GuardianStatus = status; //�μ��� ���� �����ͷ� GuardianStatus ������Ʈ

        GetComponent<SphereCollider>().radius = GuardianStatus.AttackRadius; //���ݹݰ� ������Ʈ
        GuardianRenderer.materials[0].color = GuardianStatus.Color; //Renderer �� ������Ʈ
    }

    #endregion
}