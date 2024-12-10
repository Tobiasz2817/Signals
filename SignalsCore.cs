using System.Collections.Generic;
using System.Linq.Expressions;
using CoreUtility.Extensions;
using System.Reflection;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System;

namespace Signals {
    internal static class SignalsCore {
        static Dictionary<string, SignalEvent> Events;
        const string MethodPrefix = "On";

        #region Initialize

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void InitializeEditor() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
    
        static void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (state != PlayModeStateChange.ExitingPlayMode)
                return;
            
            Events.ForEach(@event => @event.Value.Clear());                
            Events.Clear();
            Debug.Log("Clear the signals ...");
        }
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize() {
            Events = SignalsEvents();
        }


        #endregion
        #region Processing

        internal static void Registry(IList<MonoBehaviour> targets) {
            foreach (var target in targets) {
                var targetType = target.GetType();

                foreach (var methodInfo in GetMethods(targetType)) {
                    var del = CreateDelegateForMethod(target, methodInfo);
                    Events[methodInfo.Name].AddEvent(del);
                }
            }
        }
        
        internal static void RegistryMethod(object target, MethodInfo methodInfo) {
            var del = CreateDelegateForMethod(target, methodInfo);
            Events[methodInfo.Name].AddEvent(del);
        }


        internal static void RaiseSignal(string methodName, params object[] args) {
            Events[methodName.WithStart(MethodPrefix)].NotifyEvent(args);
        }
            

        #endregion
        #region Utility

        internal static IEnumerable<MethodInfo> GetMethods(Type targetType) {
            const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            
            return targetType.GetMethods(Flags).
                Where(method => method.Name.StartsWith(MethodPrefix)).
                Where(method => (Events ?? SignalsEvents()).ContainsKey(method.Name)).
                Where(method => method.EqualParameters(GetISignalMethod(method.Name), true));
        }
        
        static Dictionary<string, SignalEvent> SignalsEvents() =>
            typeof(ISignals).GetMethods()
                .ToDictionary(method => method.Name, _ => new SignalEvent());
        static MethodInfo GetISignalMethod(string methodName) =>
            typeof(ISignals).GetMethods().FirstOrDefault((m) => m.Name.Equals(methodName));
        
        static Delegate CreateDelegateForMethod(object target, MethodInfo methodInfo) {
            var parameters = methodInfo.GetParameters();
            Type delegateType = methodInfo.ReturnType == typeof(void)
                ? Expression.GetActionType(parameters.Select(p => p.ParameterType).ToArray())
                : Expression.GetFuncType(parameters.Select(p => p.ParameterType).ToArray().Add(methodInfo.ReturnType));

            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        #endregion
    }
}