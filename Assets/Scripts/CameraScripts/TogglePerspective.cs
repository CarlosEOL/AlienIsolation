
using Unity.Cinemachine;
using UnityEngine;

public class TogglePerspective :  MonoBehaviour
{
    [SerializeField] CinemachineCamera[] _cameras;

    public static int index = 0;
    
    public void Toggle()
    {
        index++;
        if (index >= _cameras.Length) index = 0;
        
        foreach (var camera in _cameras)
        {
            if (camera.enabled)
            {
                camera.enabled = false;
            }
            else
            {
                camera.enabled = true;
            }
        }
    }
}
