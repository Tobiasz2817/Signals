#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using CoreUtility;

namespace Signals {
    public static class SignalsScannerInitializer
    {
        static readonly string[] SearchFolders = new[]{
            "Assets/Content'Prefabs",
            "Assets/Art/Prefabs"
        };
        
        [MenuItem("Tools/Signals/ScanActiveScene")]
        static void SearchAndScanInActiveScene() {
            var activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(activeScene);
            ScanScene(activeScene.path);
        }
        
        [MenuItem("Tools/Signals/ScanScenes")]
        static void SearchAndScanInScenes() {
            var activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(activeScene);
            
            string activeScenePath = activeScene.path;
            ScanScene(activeScenePath);

            foreach (var editorScene in EditorBuildSettings.scenes)
            {
                if (!editorScene.enabled) 
                    continue;

                ScanScene(editorScene.path);
            }
            
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
        }

        static void ScanScene(string scenePath) {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Debug.Log($"Searching scene: {scene.name}");

            var scanner = Utility.FindOrAddInScene<SignalsScanner>(scene, false);
            if (!scanner)
                return;
                
            scanner.Scanning(true);

            if (scanner.Methods.Count <= 0) 
                Object.DestroyImmediate(scanner.gameObject, true);
            else
                SceneManager.MoveGameObjectToScene(scanner.gameObject, scene);
                
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        
        [MenuItem("Tools/Signals/ScanPrefabs")]
        static void SearchAndScanInPrefabs() {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", SearchFolders);

            foreach (string guid in prefabGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(assetPath);

                if (prefab == null) 
                    continue;

                Debug.Log($"Found prefab '{prefab.name}' at path '{assetPath}'");

                SignalsScanner scanner = prefab.GetComponentInChildren<SignalsScanner>();
                if (scanner == null) {
                    var rootGo = new GameObject("Signals");
                    rootGo.transform.SetParent(prefab.transform.root);
                    scanner = rootGo.AddComponent<SignalsScanner>();
                }
                
                scanner.Scanning(false);

                if (scanner.Methods.Count <= 0) {
                    var scannerTransform = scanner.transform;
                    Object.DestroyImmediate(scanner, true);
                    
                    // Is not root and is empty
                    if(ReferenceEquals(scannerTransform, scannerTransform.root))
                        continue;

                    if(scannerTransform.GetComponents<MonoBehaviour>().Length > 0)
                        continue;
                    
                    Object.DestroyImmediate(scannerTransform.gameObject, true);
                }

                EditorUtility.SetDirty(prefab);
                
                PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
                PrefabUtility.UnloadPrefabContents(prefab);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/Signals/ClearPrefabs")]
        static void ClearPrefabs() {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", SearchFolders);

            foreach (string guid in prefabGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab == null) 
                    continue;

                Debug.Log($"Found prefab '{prefab.name}' at path '{assetPath}'");
                
                var scanner = prefab.GetComponentInChildren<SignalsScanner>();
                if(!scanner)
                    continue;

                Object.DestroyImmediate(scanner, true);
                EditorUtility.SetDirty(prefab);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Signals/ScanAll")]
        static void RunAll() {
            SearchAndScanInScenes();
            SearchAndScanInPrefabs();
        }
    }
}
#endif
