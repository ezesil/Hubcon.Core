﻿using Castle.DynamicProxy;
using Hubcon.Extensions;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Hubcon.Handlers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MethodHandler
    {
        internal ConcurrentDictionary<string, Delegate?> AvailableMethods = new();

        public MethodHandler()
        {
           
        }

        public void BuildMethods(object instance, Type type, Action<string, MethodInfo, MethodHandler>? forEachMethodAction = null)
        {
            if (!typeof(IHubconController).IsAssignableFrom(type))
                throw new NotImplementedException($"El tipo {type.FullName} no implementa la interfaz {nameof(IHubconController)} o un tipo derivado.");

            if (AvailableMethods.IsEmpty)
            {
                var interfaces = type.GetInterfaces().Where(x => typeof(ICommunicationContract).IsAssignableFrom(x));

                foreach (var item in interfaces)
                {
                    if (item.GetMethods().Length == 0)
                        continue;

                    foreach (var method in item.GetMethods())
                    {
                        var parameters = method.GetParameters();
                        var parameterExpressions = parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

                        var callExpression = method.ReturnType == typeof(void) ?
                            Expression.Call(Expression.Constant(instance), method, parameterExpressions) :
                            (Expression)Expression.Call(Expression.Constant(instance), method, parameterExpressions);

                        Type delegateType = method.ReturnType == typeof(void) ?
                            Expression.GetActionType(parameters.Select(p => p.ParameterType).ToArray()) :
                            Expression.GetFuncType(parameters.Select(p => p.ParameterType).Concat(new Type[] { method.ReturnType }).ToArray());

                        var lambda = Expression.Lambda(delegateType, callExpression, parameterExpressions);
                        Delegate? action = lambda.Compile();

                        var methodSignature = method.GetMethodSignature();

                        AvailableMethods.TryAdd($"{methodSignature}", action);

                        forEachMethodAction?.Invoke(methodSignature, method, this);
                    }
                }
            }
        }

        public async Task HandleWithoutResultAsync(MethodInvokeRequest methodInfo)
        {
            Console.WriteLine($"[MethodHandler] Received call {methodInfo.MethodName}. Args: [{string.Join(",", methodInfo.Args.Select(x => $"{x}"))}]");

            AvailableMethods.TryGetValue(methodInfo.MethodName, out Delegate? del);
            object? result = del?.DynamicInvoke(methodInfo.GetDeserializedArgs(del));

            if (result is Task task)
                await task;
        }

        public async Task<MethodResponse> HandleSynchronousResult(MethodInvokeRequest methodInfo)
        {
            Console.WriteLine($"[MethodHandler] Received call {methodInfo.MethodName}. Args: [{string.Join(",", methodInfo.Args.Select(x => $"{x}"))}]");

            return await Task.Run(() =>
            {
                AvailableMethods.TryGetValue(methodInfo.MethodName, out Delegate? del);
                object? result = del?.DynamicInvoke(methodInfo.GetDeserializedArgs(del));

                if (result is null)
                    return new MethodResponse(true);

                return new MethodResponse(true, result).SerializeData();
            });
        }

        public Task HandleSynchronous(MethodInvokeRequest methodInfo)
        {
            Console.WriteLine($"[MethodHandler] Received call {methodInfo.MethodName}. Args: [{string.Join(",", methodInfo.Args.Select(x => $"{x}"))}]");

            AvailableMethods.TryGetValue(methodInfo.MethodName, out Delegate? del);
            del?.DynamicInvoke(methodInfo.GetDeserializedArgs(del));

            return Task.CompletedTask;
        }

        public async Task<MethodResponse> HandleWithResultAsync(MethodInvokeRequest methodInfo)
        {
            Console.WriteLine($"[MethodHandler] Received call {methodInfo.MethodName}. Args: [{string.Join(",", methodInfo.Args.Select(x => $"{x}"))}]");

            AvailableMethods.TryGetValue(methodInfo.MethodName, out Delegate? del);
            object? result = del?.DynamicInvoke(methodInfo.GetDeserializedArgs(del));

            if (result is null)
                return new MethodResponse(true);
            else if (result is Task task)
            {
                var response = await GetTaskResultAsync(task, del!.Method.ReturnType.GetGenericArguments()[0]);
                return new MethodResponse(true, response).SerializeData();
            }
            else
                return new MethodResponse(true, result).SerializeData();
        }

        public static async Task<object?> GetTaskResultAsync(Task taskObject, Type returnType)
        {
            // Esperar a que el Task termine
            await taskObject;

            // Verificar si es un Task<T> (Task con resultado)
            var taskType = taskObject.GetType();

            if (taskType.IsGenericType)
            {
                // Obtener el tipo del resultado (T)
                var resultProperty = taskType.GetProperty("Result");

                // Obtener el resultado del Task
                var result = resultProperty?.GetValue(taskObject);

                return Convert.ChangeType(result, returnType);
            }

            // Si no es un Task<T>, no hay valor que devolver
            return null;
        }
    }
}
