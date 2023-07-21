using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Player;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraFollowSetter : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        InputHandler.Instance.OnNewCharacter += SetCameraFollow;
    }

    private void SetCameraFollow(Characters.Character obj)
    {
        _virtualCamera.Follow = obj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
