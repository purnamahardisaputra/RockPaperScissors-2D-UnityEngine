using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;



public class BotDifficultyManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulty;
    [SerializeField] BotStats[] botDifficulties;
    [Header("Remote Config Parameters : ")]
    [SerializeField] bool enableRemoteConfig = false;
    [SerializeField] string difficultyKey = "Difficulty";
    struct userAttributes { };
    struct appAttributes { };
    IEnumerator Start()
    {
        // tunggu bot selesai setup
        yield return new WaitUntil(() => bot.IsReady);

        // set stats default dari diffivulty manager
        // sesuai selectedDifficulty dari manager
        var newStats = botDifficulties[selectedDifficulty];
        bot.SetStats(newStats, true);

        // ambil difficulty dari remote config kalau enabled
        if (enableRemoteConfig == false)
        {
            yield break;
        }

        // tapi tunggu sampai unity services siap
        yield return new WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn);
        // daftar dulu untuk event fetch complete
        RemoteConfigService.Instance.FetchCompleted += OnRemoteCompleted;
        //lalu fetch disini. cukup sekali di awal permainan
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    private void OnDestroy()
    {
        // destroy untuk unregister event, menghindari memory leak
        RemoteConfigService.Instance.FetchCompleted -= OnRemoteCompleted;
    }

    private void OnRemoteCompleted(ConfigResponse response)
    {
        // setiap kali data baru didapatkan ( melalui fetch ) fungsi ini akan dipanggil
        if (RemoteConfigService.Instance.appConfig.HasKey(difficultyKey) == false)
        {
            Debug.LogWarning("Remote Config does not have key " + difficultyKey);
            return;
        }
        switch (response.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("Default");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("Cached");
                break;
            case ConfigOrigin.Remote:
                selectedDifficulty = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                selectedDifficulty = Mathf.Clamp(selectedDifficulty, 0, botDifficulties.Length - 1);
                var newStats = botDifficulties[selectedDifficulty];
                bot.SetStats(newStats, true);
                break;
        }
    }
}
