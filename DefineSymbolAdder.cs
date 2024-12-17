#if UNITY_EDITOR
using CoreUtility;
using UnityEditor;

namespace Signals {
    [InitializeOnLoad]
    public class DefineSymbolAdder {
        const string SignalsSymbol = "SIGNALS";
        
        static DefineSymbolAdder() =>
            Utility.TryAddDefineSymbol(SignalsSymbol);
    }
}
#endif