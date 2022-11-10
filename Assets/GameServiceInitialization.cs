using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class GameServiceInitialization : MonoBehaviour
{
    [SerializeField] string environmentName;
    async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Uninitialized)
            return;
        var options = new InitializationOptions();
        options.SetEnvironmentName(environmentName);
        await UnityServices.InitializeAsync(options);

        if(AuthenticationService.Instance.IsSignedIn == false)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
