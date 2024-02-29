using EnemyEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveInfo", menuName = "Scriptable Object/WaveInfo")]
public class WaveInfo : ScriptableObject
{
    public int Wave = 0;

    public float SpawnCycleTime = 3;

    public List<EnemyType> EnemyList = new List<EnemyType>();

}

[Serializable]
public class EnemyType
{
    public string Name;

    public int Count;

    public EnemyEnum.EnemyEnum Type;
    public GameObject EnemyPrefab;
}

public class EnemySpawner : MonoBehaviour
{
    public Transform SpawnPosition; //처음 Enemy가 스폰될 위치
    public GameObject[] WayPoints; //Enemy가 지나갈 포인트
    public float SpawnCycleTime = 1f; //스폰 주기

    public GameObject WaveUI;

    public WaveInfo[] WaveInfo;

    private List<GameObject> _waveSpawnEnemy = new List<GameObject>();

    public int _spawnCount;

    public Dictionary<int,int> a = new Dictionary<int,int>();

    private void Start()
    {
        Activate();
    }

    public void Activate()
    {
        StartCoroutine(SpawnEnemy(0)); //SpawnEnemy코루틴 실행
    }

    public void DeActivate()
    {
        StopCoroutine("SpawnEnemy");//SpawnEnemy코루틴 중지
    }

    IEnumerator SpawnEnemy(int Wave)
    {
        WaveUI.GetComponentInChildren<TextMeshProUGUI>().text = "Wave - " + (Wave + 1);
        WaveUI.SetActive(true);



        foreach (EnemyType type in WaveInfo[Wave].EnemyList)
        {
            for(int i = 0; i < type.Count; i++)
            {
                _spawnCount++;
                _waveSpawnEnemy.Add(type.EnemyPrefab);
                Debug.Log("A");
            }
        }

        SpawnCycleTime = WaveInfo[Wave].SpawnCycleTime;


        yield return new WaitForSeconds(2.5f); //쿨타임만큼 대기

        WaveUI.SetActive(false);


        while (_spawnCount > 0) //소환할 수 있는 동안
        {
            int index = UnityEngine.Random.Range(0, _spawnCount);
            GameObject EnemyInst = Instantiate(_waveSpawnEnemy[index], SpawnPosition.position, Quaternion.identity); //SpawnPosition위치에 Enemy 프리팹 생성
            Enemy EnemyCom = EnemyInst.GetComponent<Enemy>(); //생성한 오브젝트의 Enemy클래스 할당
            if (EnemyCom)
            {
                EnemyCom.WayPoints = WayPoints; //트랜스폼 배열 초기화
            }

            _waveSpawnEnemy.RemoveAt(index);
            _spawnCount--;

            yield return new WaitForSeconds(SpawnCycleTime); //쿨타임만큼 대기
        }

        yield return new WaitForSeconds(SpawnCycleTime);

        if (Wave + 1 < WaveInfo.Length)
        {
            Wave += 1;

            StartCoroutine(SpawnEnemy(Wave));
        }
    }

}
