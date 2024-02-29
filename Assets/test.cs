using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private void Start()
    {
        int asd = 0;

        OutFunction(out asd);
        refFunction(ref asd);
    }

    public static void OutFunction(out int b)
    {
        b = 1;
        return;
    }
    public static void refFunction(ref int b)
    {
        b += 1;
        return;
    }
}


