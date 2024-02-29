using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int Coin = 100; //현재 코인
    private int _heart = 10; //체력

    public int Heart //체력 getter
    {
        get { return _heart; }
    }
    public int MaxHeart = 10; //최대 체력

    void Start()
    {
        _heart = MaxHeart; //체력 초기화
    }

    public void Damaged(int damage)
    {
        _heart -= damage; //damage만큼 체력 감소
        if (_heart <= 0) //체력이 0 이하라면
        {
            GameManager.Inst.GameDefeat(); //게임종료 함수 실행
        }
    }
    public void UseCoin(int coin)
    {
        Coin = Mathf.Clamp(Coin - coin, 0, int.MaxValue); //0과 int 최댓값 사이에서 코인 감소
    }

    public bool CanUseCoin(int coin)
    {
        return Coin >= coin; //사용할 코인보다 현재 코인이 많은지 반환
    }

}
