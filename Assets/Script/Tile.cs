using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Guardian OwnGuardian; //해당 타일의 가디언

    public bool CheckIsOwned()
    {
        return OwnGuardian != null; //가디언이 있는지 반환
    }

    public void ClearOwned()
    {
        OwnGuardian = null; //가디언 할당 해제
    }

    public void RemoveOwned()
    {
        Destroy(OwnGuardian); //가디언 파괴
        OwnGuardian = null; //가디언 할당 해제
    }
}
