using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one {nameof(ScreenShake)}. {this.gameObject.name} and {Instance.gameObject.name}");
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
