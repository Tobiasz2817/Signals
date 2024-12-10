#if UNITY_EDITOR
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