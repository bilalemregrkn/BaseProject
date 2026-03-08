using System;
using System.Collections.Generic;
using Plugins.EventBus;
using Plugins.UpdateManager;
using Reflex.Attributes;
using UnityEngine;

public class Greeter : MonoBehaviour
{
    [Inject] private IEventBus _eventBus;
    [Inject] private IUpdateManager _updateManager;

    private void Start()
    {
    }
}