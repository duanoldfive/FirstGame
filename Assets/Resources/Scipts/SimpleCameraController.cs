using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;
using Pixeye.Unity;
using UnityEngine.InputSystem;

public class SimpleCameraController : MonoBehaviour
{
    [Foldout("BaseSetup", true)]
    public Transform _Anchor = null;
    public Vector3 AnchorOffset; //相对于目标对象的偏移
    public float Distance; //摄像机距离目标点距离
    
    [Foldout("RotationSetup", true)]
    public float Yaw;      //摄像机偏航角
    public float YawSpeed;


    [Foldout("OtherSetup", true)]
    public float FollowTime;        //用多久差值到目的地
    
    [NonSerialized]
    public Transform _Transform = null;
    [NonSerialized]
    [HideInInspector]
    public Camera _Camera;

    private float Pitch;    //摄像机俯仰角
    private Vector3 _LookatPoint;   //当前正在看的点
    private float _DistanceDelta;
    private float _YawDelta;
    
    //缓存防止每帧都new
    private Vector3 _XOffsetVector = Vector3.zero;
    private Vector3 _lookatVelocity = Vector3.zero;

    private void Awake()
    {
        _Camera = GetComponent<Camera>();

        //PlayerInput input = _Anchor.GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {   
        //缓存transform避免每次都访问,提高效率
        _Transform = transform;
        _DistanceDelta = 0f;
        _YawDelta = 0f;
    }

    //更新完成以后，再计算摄像机位置，旋转以便平滑
    private void LateUpdate()
    {
        if (_Anchor != null)
        {
            _Transform.position = GetTargetPosition(GetLookatPoint());
        }
    }

    //获得目视目标点
    private Vector3 GetLookatPoint()
    {
        Vector3 lookat = _Anchor.position + AnchorOffset;
        return lookat;
    }

    //获得摄像机的目标点
    private Vector3 GetTargetPosition(Vector3 lookat)
    {
        Vector3 dir = Vector3.back * Distance;
        dir = Quaternion.AngleAxis(Pitch, Vector3.right) * dir;
        dir = Quaternion.AngleAxis(Yaw, Vector3.up) * dir;
        return lookat + dir;
    }
}


