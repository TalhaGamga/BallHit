using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    [SerializeField] Ease ease;

    [SerializeField] float YChange = 10;

    private void OnEnable()
    {
        EventManager.OnSettingHeight += SetHeight;
        
    }

    private void OnDisable()
    {
        EventManager.OnSettingHeight -= SetHeight;
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 60*Time.deltaTime);    
    }


    private void SetHeight()
    { 
        WallBase wall = EventManager.OnGettingPeek?.Invoke();
        transform.DOMoveY(wall.transform.position.y + YChange,1f);
    }
}
