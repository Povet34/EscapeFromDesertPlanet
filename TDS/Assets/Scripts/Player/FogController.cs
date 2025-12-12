using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogController : MonoBehaviour
{
    [SerializeField] VolumetricFog fogOfWarFog;
    [SerializeField] VolumetricFog sendStormFog;

    public void FixedUpdate()
    {
        transform.position = GameManager.instance.player.transform.position;
    }
}
