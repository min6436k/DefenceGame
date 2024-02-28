using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Guardian OwnGuardian; //�ش� Ÿ���� �����

    public bool CheckIsOwned()
    {
        return OwnGuardian != null; //������� �ִ��� ��ȯ
    }

    public void ClearOwned()
    {
        OwnGuardian = null; //����� �Ҵ� ����
    }

    public void RemoveOwned()
    {
        Destroy(OwnGuardian); //����� �ı�
        OwnGuardian = null; //����� �Ҵ� ����
    }
}
