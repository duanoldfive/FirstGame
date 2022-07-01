using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator animtor;
    
    public void OnSpeedChanged(float value)
    {
        animtor.SetFloat("speed", value);   
    }

    public void OnDirXChanged(float value)
    {
        animtor.SetFloat("dirX", value);
    }

    public void OnDirYChanged(float value)
    {
        animtor.SetFloat("dirY", value);
    }
}
