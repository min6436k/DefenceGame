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
    public Transform SpawnPosition; //ó�� Enemy�� ������ ��ġ
    public GameObject[] WayPoints; //Enemy�� ������ ����Ʈ
    public float SpawnCycleTime = 1f; //���� �ֱ�

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
        StartCoroutine(SpawnEnemy(0)); //SpawnEnemy�ڷ�ƾ ����
    }

    public void DeActivate()
    {
        StopCoroutine("SpawnEnemy");//SpawnEnemy�ڷ�ƾ ����
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


        yield return new WaitForSeconds(2.5f); //��Ÿ�Ӹ�ŭ ���

        WaveUI.SetActive(false);


        while (_spawnCount > 0) //��ȯ�� �� �ִ� ����
        {
            int index = UnityEngine.Random.Range(0, _spawnCount);
            GameObject EnemyInst = Instantiate(_waveSpawnEnemy[index], SpawnPosition.position, Quaternion.identity); //SpawnPosition��ġ�� Enemy ������ ����
            Enemy EnemyCom = EnemyInst.GetComponent<Enemy>(); //������ ������Ʈ�� EnemyŬ���� �Ҵ�
            if (EnemyCom)
            {
                EnemyCom.WayPoints = WayPoints; //Ʈ������ �迭 �ʱ�ȭ
            }

            _waveSpawnEnemy.RemoveAt(index);
            _spawnCount--;

            yield return new WaitForSeconds(SpawnCycleTime); //��Ÿ�Ӹ�ŭ ���
        }

        yield return new WaitForSeconds(SpawnCycleTime);

        if (Wave + 1 < WaveInfo.Length)
        {
            Wave += 1;

            StartCoroutine(SpawnEnemy(Wave));
        }
    }

}
