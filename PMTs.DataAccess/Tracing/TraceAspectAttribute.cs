//using AspectInjector.Broker;
//using DataAccess.Utils;
//using OpenTelemetry.Trace;
//using System.Diagnostics;

namespace DataAccess.Tracing
{
    //[Aspect(Scope.Global)]
    //[Injection(typeof(TraceAspectAttribute))]
    //public sealed class TraceAspectAttribute : Attribute
    //{

    //    [Advice(Kind.Before, Targets = Target.Method)]
    //    public void TraceStart([Argument(Source.Type)] Type type, [Argument(Source.Name)] string name)
    //    {
    //        using Activity? activity = ActivitySourceProvider.Source!.StartActivity($"{type.Name}::{name}");
    //    }

    //    [Advice(Kind.After, Targets = Target.Method)]
    //    public void TraceFinish([Argument(Source.Type)] Type type, [Argument(Source.Name)] string name)
    //    {
    //    }
    //}
}
