using System;

namespace Signals {
    class SignalEvent {
        Delegate _delegate;
        
        public void NotifyEvent(params object[] args) =>
            _delegate?.DynamicInvoke(args);
        public void AddEvent(Delegate del) =>
            _delegate = Delegate.Combine(_delegate, del);
        public void RemoveEvent(Delegate del) =>
            _delegate = Delegate.Remove(_delegate, del);
        public void Clear() =>
            _delegate = null;
    }
}