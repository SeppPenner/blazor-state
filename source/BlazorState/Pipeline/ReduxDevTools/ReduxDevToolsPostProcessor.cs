﻿namespace BlazorState.Pipeline.ReduxDevTools
{
  using BlazorState;
  using MediatR;
  using MediatR.Pipeline;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  //TODO: this should be a IRequestPostProcessor but I couldn't get it to work.

  /// <summary>
  ///
  /// </summary>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  public class ReduxDevToolsPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
  {
    private ILogger Logger { get; }

    private ReduxDevToolsInterop ReduxDevToolsInterop { get; }

    //private History<TState> History { get; }
    private IReduxDevToolsStore Store { get; }

    public ReduxDevToolsPostProcessor
    (
      ILogger<ReduxDevToolsPostProcessor<TRequest, TResponse>> aLogger,
      ReduxDevToolsInterop aReduxDevToolsInterop,
      IReduxDevToolsStore aStore
    )
    {
      Logger = aLogger;
      Logger.LogDebug($"{GetType().Name} constructor");
      Store = aStore;
      ReduxDevToolsInterop = aReduxDevToolsInterop;
    }


    public Task Process(TRequest aRequest, TResponse aResponse, CancellationToken aCancellationToken)
    {
      try
      {
        Logger.LogDebug($"{GetType().Name}: Start Post Processing");
        if (!(aRequest is IReduxRequest))
        {
          ReduxDevToolsInterop.Dispatch(aRequest, Store.GetSerializableState());
        }
        Logger.LogDebug($"{GetType().Name}: End");
      }
      catch (Exception e)
      {
        Logger.LogDebug($"{GetType().Name}: Error: {e.Message}");
        Logger.LogDebug($"{GetType().Name}: InnerException: {e.InnerException?.Message}");
        Logger.LogDebug($"{GetType().Name}: StackTrace: {e.StackTrace}");
        throw;
      }
      return Task.CompletedTask;
    }
  }
}