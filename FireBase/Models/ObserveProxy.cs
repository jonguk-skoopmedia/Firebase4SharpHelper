using System;
using System.Threading.Tasks;

namespace FirebaseSharp.Firebase.Models
{
    public class InvokeEventArgs : EventArgs
    {
        public InvokeEventArgs(object result)
        {
            Result = result;
        }

        public object Result { get; private set; }
    }

    public class ObserveProxy
    {
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async Task<object> InvokeFromFirebase(object input)
#pragma warning restore CS1998
        {
            try
            {
                OnInvokeFromFirebase(input);
            }
            catch (Exception e)
            {
                // TODO: logging
            }

            return true;
        }

        public static event EventHandler<InvokeEventArgs> InvokedFromFDBObserve;
        private static void OnInvokeFromFirebase(object input)
        {
            InvokedFromFDBObserve?.Invoke(null, new InvokeEventArgs(input));
        }
    }
}
