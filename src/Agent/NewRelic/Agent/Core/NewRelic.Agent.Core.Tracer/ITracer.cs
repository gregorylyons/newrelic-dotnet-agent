using System;
using NewRelic.Agent.Core.Utilities;
using NewRelic.Agent.Core.WireModels;

namespace NewRelic.Agent.Core.Tracer
{

    /// <summary>
    /// Represents the most basic method tracer.
    /// The bytecode injected around traced methods will call Agent.FinishTracer,
    /// which in turn calls the Finish method.
    /// This layer of indirection is needed because of the CLR security model.
    /// </summary>
    public interface ITracer
    {
        /// <summary>
        /// Called (indirectly through Agent.FinishTracer) after the invocation of a traced method completes.
        /// </summary>
        /// <param name="returnValue">
        /// The object returned by the invocation.
        /// </param>
        /// <param name="exception">
        /// The exception thrown by the invocation.
        /// </param>
        void Finish(Object returnValue, Exception exception);
    }
}
