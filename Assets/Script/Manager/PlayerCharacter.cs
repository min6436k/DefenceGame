using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int Coin = 100;
    private int _heart = 10;

    public int Heart
    {
        get { return _heart; }
    }
    public int MaxHeart = 10;

    void Start()
    {
        _heart = MaxHeart;
    }

    public void Damaged(int damage)
    {
        _heart -= damage;
        if(_heart <= 0)
        {
            GameManager.Inst.GameDefeat();
        }
    }
    public void UseCoin(int coin)
    {
        Coin = Mathf.Clamp(Coin - coin, 0, int.MaxValue);
    }

    public bool CanUseCoin(int coin)
    {
        return Coin >= coin;
    }

}
