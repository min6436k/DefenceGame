using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform SpawnPosition; //처음 Enemy가 스폰될 위치
    public GameObject[] WayPoints; //Enemy가 지나갈 포인트
    public GameObject EnemyPrefab; 
    public float SpawnCycleTime = 1f; //스폰 주기

    private bool _bCanSpawn = true;

    private void Start()
    {
        Activate();
    }

    public void Activate()
    {
        StartCoroutine(SpawnEnemy()); //SpawnEnemy코루틴 실행
    }

    public void DeActivate()
    {
        StopCoroutine(SpawnEnemy()); //SpawnEnemy코루틴 중지
    }

    IEnumerator SpawnEnemy()
    {
        while (_bCanSpawn) //소환할 수 있는 동안
        {
            yield return new WaitForSeconds(SpawnCycleTime); //쿨타임만큼 대기

            GameObject EnemyInst = Instantiate(EnemyPrefab, SpawnPosition.position, Quaternion.identity); //SpawnPosition위치에 Enemy 프리팹 생성
            Enemy EnemyCom = EnemyInst.GetComponent<Enemy>(); //생성한 오브젝트의 Enemy클래스 할당
            if (EnemyCom) 
            {
                EnemyCom.WayPoints = WayPoints; //트랜스폼 배열 초기화
            }
        }
    }

}
