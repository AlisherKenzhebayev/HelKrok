using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerDamageTaker : DamageTaker
{
    CinemachineVirtualCamera vcam;
    float fovMultiplier = 100f;


    private void Awake()
    {
        vcam = GameObject.FindGameObjectWithTag("Player").transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();
        print(vcam);
    }
    private void Update()
    {
        EventManager.TriggerEvent("currentHealthPlayer", new Dictionary<string, object> { { "amount", this.FracHealth } });
    }

    internal override void DoDeath()
    {
        //GameplayManager.ShowUiScreen();
        StartCoroutine(DeathAfterTime());
        //GameplayManager.SaveGame();
        //SceneLoaderManager.LoadEnum(SceneLoaderManager.ScenesEnum.DeathScene);
    }

    IEnumerator DeathAfterTime(float time = 1f)
    {
        float startFOV = vcam.m_Lens.FieldOfView;
        for (float timer = 0; timer < time; timer += Time.deltaTime)
        {
            yield return null;
            vcam.m_Lens.FieldOfView -= Time.deltaTime * fovMultiplier;
        }


        GameplayManager.ResetCheckpoint();
        for (float timer = 0; timer < time; timer += Time.deltaTime)
        {
            yield return null;
            vcam.m_Lens.FieldOfView += Time.deltaTime * fovMultiplier;
        }

        vcam.m_Lens.FieldOfView = startFOV;
    }


}