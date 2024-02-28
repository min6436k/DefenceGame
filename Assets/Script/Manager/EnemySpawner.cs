using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform SpawnPosition; //ó�� Enemy�� ������ ��ġ
    public GameObject[] WayPoints; //Enemy�� ������ ����Ʈ
    public GameObject EnemyPrefab; 
    public float SpawnCycleTime = 1f; //���� �ֱ�

    private bool _bCanSpawn = true;

    private void Start()
    {
        Activate();
    }

    public void Activate()
    {
        StartCoroutine(SpawnEnemy()); //SpawnEnemy�ڷ�ƾ ����
    }

    public void DeActivate()
    {
        StopCoroutine(SpawnEnemy()); //SpawnEnemy�ڷ�ƾ ����
    }

    IEnumerator SpawnEnemy()
    {
        while (_bCanSpawn) //��ȯ�� �� �ִ� ����
        {
            yield return new WaitForSeconds(SpawnCycleTime); //��Ÿ�Ӹ�ŭ ���

            GameObject EnemyInst = Instantiate(EnemyPrefab, SpawnPosition.position, Quaternion.identity); //SpawnPosition��ġ�� Enemy ������ ����
            Enemy EnemyCom = EnemyInst.GetComponent<Enemy>(); //������ ������Ʈ�� EnemyŬ���� �Ҵ�
            if (EnemyCom) 
            {
                EnemyCom.WayPoints = WayPoints; //Ʈ������ �迭 �ʱ�ȭ
            }
        }
    }

}
