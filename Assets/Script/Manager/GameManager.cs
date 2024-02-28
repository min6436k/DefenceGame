using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst
    {
        get; private set;
    }
    public PlayerCharacter playerCharacter;
    public GuardianUpgradeManager guardianUpgradeManager;
    public GuardianBuildManager guardianBuildManager;

    private void Awake()
    {
        if(Inst == null) //싱글톤 패턴
        {
            Inst = this;
        }
        else
        {
            Destroy(Inst);
        }
    }

    public void GameDefeat()
    {

    }
    public void EnemyDead(int coin)
    {
        playerCharacter.Coin += coin; //플레이어의 코인 증가
    }
}
