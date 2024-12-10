



namespace Signals {
    public static class BusSignals {
        public static void MovementCondition(bool condition) => SignalsCore.RaiseSignal(nameof(MovementCondition), condition);
        public static void DamageDeal(float damage) => SignalsCore.RaiseSignal(nameof(DamageDeal), damage);
        public static void CharacterJump(int jumpCount) => SignalsCore.RaiseSignal(nameof(CharacterJump), jumpCount);
    }

    // Must be prefix "On"
    public interface ISignals {
        void OnMovementCondition(bool condition);
        void OnDamageDeal(float damage);
        void OnCharacterJump(int jumpCount);
    }
}