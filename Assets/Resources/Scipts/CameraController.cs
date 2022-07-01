using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;
using Pixeye.Unity;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Foldout("BaseSetup", true)]
    public Transform _Anchor = null;
    public Vector3 AnchorOffset; //相对于目标对象的偏移
    public float Distance; //摄像机距离目标点距离
    public float MinDistance;   //距离范围
    public float MaxDistance;
    public float ZoomSpeed;
    public float MinPitch;  //俯仰角范围
    public float MaxPitch;
    public AnimationCurve PitchCurve;

    [Foldout("RotationSetup", true)]
    public bool CanRotation;
    [HideInInspector]
    public float Yaw;      //摄像机偏航角
    public float YawSpeed;


    [Foldout("OtherSetup", true)]
    public float FollowTime;        //用多久差值到目的地
    public float FollowDistance;    //多远的距离开始跟随
    public bool CanZoom;        //是否可以缩放

    [NonSerialized]
    public Transform _Transform = null;
    public virtual Transform Transform
    {
        get { return _Transform; }
    }

    /// <summary>
    /// The camera that is mounted on the rig. This is the 'lens'
    /// that we'll actually see through.
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public Camera _Camera;
    public virtual Camera Camera
    {
        get { return _Camera; }
    }

    //跟随的角色
    public virtual Transform Anchor
    {
        get { return _Anchor; }
        set { _Anchor = value; }
    }

    
    private float ModifyYawDelta = 0f; //其他模块输入每帧修改旋转差量，每次生效后清零，外部重新设置，若接收到操作输入，该值就会失效
    private float Pitch;    //摄像机俯仰角

    private Vector3 _LookatPoint;   //当前正在看的点
    public Vector3 LookatPoint
    {
        get => _LookatPoint;
    }

    private bool _CameraMoving;     //是否正在移动(超出_FollowDistance才开始)
    private float _DistanceDelta;
    private float _YawDelta;
    
    //缓存防止每帧都new
    private Vector3 _XOffsetVector = Vector3.zero;
    private Vector3 _lookatVelocity = Vector3.zero;

    private void Awake()
    {
        _Camera = GetComponent<Camera>();

        PlayerInput input = _Anchor.GetComponent<PlayerInput>();
    }

    void OnZoom(float delta)
    {
        if (CanZoom)
            _DistanceDelta = -delta * ZoomSpeed * Time.unscaledDeltaTime;
    }


    void OnRotation(float delta)
    {
        if (CanRotation)
        {
            _YawDelta = delta * YawSpeed * Time.unscaledDeltaTime;
        }
    }

    private void OnDestroy()
    {
        //InputMgr.Inst.ZoomAction -= OnZoom;
        //InputMgr.Inst.RotationAction -= OnRotation;
    }



    // Start is called before the first frame update
    void Start()
    {   
        //缓存transform避免每次都访问,提高效率
        _Transform = transform;
        _CameraMoving = true;
        _DistanceDelta = 0f;
        _YawDelta = 0f;
    }

    //更新完成以后，再计算摄像机位置，旋转以便平滑
    private void LateUpdate()
    {
        if (_Anchor != null)
        {
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

    //设置跟随目标
    public void SetAnchor(Transform anchor)
    {
        _Anchor = anchor;
        _LookatPoint = GetLookatPoint();
    }

    //根据距离，用曲线算出俯仰角
    private float RefreshPitch()
    {
        float time = (Distance - MinDistance) / (MaxDistance - MinDistance);
        Pitch = MinPitch + (MaxPitch - MinPitch) * PitchCurve.Evaluate(time);
        return Pitch;
    }
}


