﻿namespace BlazorState
{
  using System;
  using System.Collections.Generic;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.DependencyInjection.Extensions;
  using Microsoft.Extensions.DependencyInjection;

  /// <summary>
  /// </summary>
  internal partial class Store : IStore
  {
    private readonly IServiceProvider ServiceProvider;

    public Store
    (
      ILogger<Store> aLogger,
      IServiceProvider aServiceProvider
    )
    {
      Logger = aLogger;
      ServiceProvider = aServiceProvider;
      
      using (Logger.BeginScope(new Dictionary<string, object> { [nameof(Guid)] = Guid }))
      {
        Logger.LogInformation($"{GetType().Name}: constructor: {nameof(Guid)}:{Guid}");
        States = new Dictionary<string, IState>();
      }
    }

    /// <summary>
    /// Unique Guid for the Store.
    /// </summary>
    /// <remarks>Useful when logging </remarks>
    public Guid Guid { get; } = Guid.NewGuid();

    private ILogger Logger { get; }
    private IDictionary<string, IState> States { get; }

    /// <summary>
    /// Get the State of the particular type
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <returns>The specific IState</returns>
    public TState GetState<TState>()
    {
      Type stateType = typeof(TState);
      return (TState)GetState(stateType);
    }

    /// <summary>
    /// Clear all the states
    /// </summary>
    public void Reset() => States.Clear();

    /// <summary>
    /// Set the state for specific Type
    /// </summary>
    /// <param name="aNewState"></param>
    public void SetState(IState aNewState)
    {
      string typeName = aNewState.GetType().FullName;
      SetState(typeName, aNewState);
    }

    private object GetState(Type aType)
    {
      using (Logger.BeginScope(nameof(GetState)))
      {
        string typeName = aType.FullName;
        Logger.LogDebug($"{GetType().Name}: {nameof(this.GetState)} typeName:{typeName}");

        if (!States.TryGetValue(typeName, out IState state))
        {
          Logger.LogDebug($"{GetType().Name}: Creating State of type: {typeName}");
          state = (IState) ServiceProvider.GetService(aType);
          if (state == null) throw new NullReferenceException("state is null");
          //state = (IState)Activator.CreateInstance(aType);
          States.Add(typeName, state);
        }
        else
          Logger.LogDebug($"{GetType().Name}: State exists: {state.Guid}");
        return state;
      }
    }

    private void SetState(string aTypeName, object aNewState)
    {
      var newState = (IState)aNewState;
      Logger.LogDebug($"{GetType().Name}: {nameof(SetState)}: typeName:{aTypeName}: Guid:{newState.Guid}");
      States[aTypeName] = newState;
    }


  }
}