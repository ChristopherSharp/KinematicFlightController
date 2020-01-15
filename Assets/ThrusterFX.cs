using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ThrusterFX : MonoBehaviour
{
    public bool IsMax;

    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _emissionModule;
    private FlightControlSystem _flightControlSystem;
    private void Awake()
    {
        _flightControlSystem = GetComponentInParent<FlightControlSystem>();
        _particleSystem = GetComponent<ParticleSystem>();
        _emissionModule = _particleSystem.emission;

        StartCoroutine(FxCoroutine());
    }

    private IEnumerator FxCoroutine()
    {
        while (!_flightControlSystem.HasLanded)
        {
            _emissionModule.enabled = IsMax == _flightControlSystem.Output > 0;

            yield return null;
        }

        _emissionModule.enabled = false;
    }
}
