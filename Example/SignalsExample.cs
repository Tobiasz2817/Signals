using UnityEngine;

namespace Signals.Example {
    public class SignalsExample : MonoBehaviour {
        public float dmg = 5f;
        
        void Awake() =>
            BusSignals.MovementCondition(false);

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) 
                BusSignals.DamageDeal(dmg);
        }
        
        // Methods processed by signals
        void OnMovementCondition(bool condition) =>
            Debug.Log("OnMovementCondition: " + condition);
        void OnDamageDeal(float damage) =>
            Debug.Log("OnDamageDeal: " + damage);
    }
}