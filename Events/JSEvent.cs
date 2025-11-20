using System;
using UnityJS.Payloads;

namespace UnityJS.Events
{
    public interface IJSEvent<TTarget>
    {
        ResponsePayload Invoke(TTarget target, string payload);
    }
    public abstract class JSEvent<TTarget, TPayload, TResult> : IJSEvent<TTarget>
    {
        protected abstract TResult Invoke(TTarget target, TPayload payload);
        ResponsePayload IJSEvent<TTarget>.Invoke(TTarget target, string rawPayload)
        {
            if (!PayloadParser.TryParse<TPayload>(rawPayload, out var payload)) 
                return ResponsePayload.Error("Failed to parse payload.\n" + rawPayload);

            var result = Invoke(target, payload);
            if(!PayloadParser.TryStringify(result, out var resultJson))
                return ResponsePayload.Error("Failed to stringify result.");
            
            return ResponsePayload.Ok(resultJson);
        }
    }
    
    public abstract class JSEventVoid<TTarget, TPayload> : IJSEvent<TTarget>
    {
        protected abstract void Invoke(TTarget target, TPayload payload);
        ResponsePayload IJSEvent<TTarget>.Invoke(TTarget target, string rawPayload)
        {
            if (!PayloadParser.TryParse<TPayload>(rawPayload, out var payload))
                return ResponsePayload.Error("Failed to parse payload.\n" + rawPayload);
            
            Invoke(target, payload);
            return ResponsePayload.Ok();
        }
    }
}