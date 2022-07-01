using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleActorController : MonoBehaviour
{
    public float moveSpeed; //角色移动速度
    
    private InputAction m_MoveAction;
    private Animator animator;
    private Vector3 rotationVector;

    
    private Vector2 m_Move;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (m_MoveAction == null)
        {
            PlayerInput input = GetComponent<PlayerInput>();
            m_MoveAction = input.actions["move"];
        }
    }

    private void Update()
    {
        MoveActor();
    }

    //移动角色
    public void MoveActor()
    {
        bool isMoving = false;
        if (m_MoveAction != null && m_MoveAction.IsPressed())
        {
            m_Move = m_MoveAction.ReadValue<Vector2>();
            if(m_Move.sqrMagnitude > 0.1f)
            {
                Vector2 moveVector = m_Move * (moveSpeed * Time.deltaTime);
                //求前进的方向
                Vector3 dir = Camera.main.transform.localToWorldMatrix.MultiplyVector(new Vector3(moveVector.x, 0, moveVector.y));
                dir.y = 0f;
                dir = dir.normalized;
                //把角色转向前进方向
                transform.forward = Vector3.SmoothDamp(transform.forward, dir, ref rotationVector, 0.1f);
                transform.position = transform.position + dir * moveVector.magnitude;
                isMoving = true;
            }
        }
        
        animator.SetFloat("speed", isMoving ? moveSpeed : 0.0f);
    }
}
