﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using NewRelic.Agent.Extensions.Providers.Wrapper;
using NewRelic.Reflection;
using System;
using System.Threading.Tasks;

namespace NewRelic.Providers.Wrapper.AspNetCore
{
	public class InvokeActionMethodAsyncWrapper : IWrapper
	{
		private Func<object, ControllerContext> _getControllerContext;
		private Func<object, ControllerContext> GetControllerContext(string typeName) { return _getControllerContext ?? (_getControllerContext = VisibilityBypasser.Instance.GenerateFieldAccessor<ControllerContext>("Microsoft.AspNetCore.Mvc.Core", typeName, "_controllerContext")); }

		public bool IsTransactionRequired => true;

		public CanWrapResponse CanWrap(InstrumentedMethodInfo methodInfo)
		{
			return new CanWrapResponse("NewRelic.Providers.Wrapper.AspNetCore.InvokeActionMethodAsync".Equals(methodInfo.RequestedWrapperName));
		}

		public AfterWrappedMethodDelegate BeforeWrappedMethod(InstrumentedMethodCall instrumentedMethodCall, IAgentWrapperApi agentWrapperApi, ITransactionWrapperApi transactionWrapperApi)
		{
			if (instrumentedMethodCall.IsAsync)
			{
				transactionWrapperApi.AttachToAsync();
			}

			//handle the .NetCore 3.0 case where the namespace is Infrastructure instead of Internal.
			var controllerContext = GetControllerContext(instrumentedMethodCall.MethodCall.Method.Type.FullName).Invoke(instrumentedMethodCall.MethodCall.InvocationTarget);
			var actionDescriptor = controllerContext.ActionDescriptor;

			var transactionName = CreateTransactionName(actionDescriptor);

			transactionWrapperApi.SetWebTransactionName(WebTransactionType.MVC, transactionName, TransactionNamePriority.FrameworkHigh);

			//Framework uses ControllerType.Action for these metrics & transactions. WebApi is Controller.Action for both
			//Taking opinionated stance to do ControllerType.MethodName for segments. Controller/Action for transactions
			var controllerTypeName = controllerContext.ActionDescriptor.ControllerTypeInfo.Name;
			var methodName = controllerContext.ActionDescriptor.MethodInfo.Name;

			var segment = transactionWrapperApi.StartMethodSegment(instrumentedMethodCall.MethodCall, controllerTypeName, methodName);

			return Delegates.GetDelegateFor<Task>(
				onFailure: segment.End,
				onSuccess: HandleSuccess
			);

			void HandleSuccess(Task task)
			{
				//TODO: This is littered throughout all async code... 
				//is it necessary to prevent stacking w/ synchronous calls that follow?
				//how are further calls appropriately stacked?
				segment.RemoveSegmentFromCallStack();

				if (task == null)
				{
					return;
				}
				
				task.ContinueWith(OnTaskCompletion);
			}

			void OnTaskCompletion(Task completedTask)
			{
				try
				{
					segment.End();
				}
				catch (Exception ex)
				{
					agentWrapperApi.SafeHandleException(ex);
				}
			}
		}

		private static string CreateTransactionName(ControllerActionDescriptor actionDescriptor)
		{
			var controllerName = actionDescriptor.ControllerName;
			var actionName = actionDescriptor.ActionName;

			var transactionName = $"{controllerName}/{actionName}";

			foreach (var parameter in actionDescriptor.Parameters)
			{
				transactionName += "/{" + parameter.Name + "}";
			}

			return transactionName;
		}
	}
}
