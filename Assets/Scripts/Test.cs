using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
public class Test : MonoBehaviour
{
    public ComputeShader testshader;
    public ComputeBuffer buffer;



    private void Start()
    {
        buffer = new ComputeBuffer(1, sizeof(float));
        testshader.SetBuffer(0, "Result", buffer);
        testshader.Dispatch(0,1, 1, 1);
        float[] arr = new float[1];
        buffer.GetData(arr);
        print(arr[0]);
    }
}
