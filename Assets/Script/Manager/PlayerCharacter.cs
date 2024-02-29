using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int Coin = 100; //���� ����
    private int _heart = 10; //ü��

    public int Heart //ü�� getter
    {
        get { return _heart; }
    }
    public int MaxHeart = 10; //�ִ� ü��

    void Start()
    {
        _heart = MaxHeart; //ü�� �ʱ�ȭ
    }

    public void Damaged(int damage)
    {
        _heart -= damage; //damage��ŭ ü�� ����
        if (_heart <= 0) //ü���� 0 ���϶��
        {
            GameManager.Inst.GameDefeat(); //�������� �Լ� ����
        }
    }
    public void UseCoin(int coin)
    {
        Coin = Mathf.Clamp(Coin - coin, 0, int.MaxValue); //0�� int �ִ� ���̿��� ���� ����
    }

    public bool CanUseCoin(int coin)
    {
        return Coin >= coin; //����� ���κ��� ���� ������ ������ ��ȯ
    }

}
