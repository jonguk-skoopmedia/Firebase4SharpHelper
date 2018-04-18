using System.Threading;
using System.Threading.Tasks;

namespace FirebaseSharp.FireBase.Extensions
{
    public static class WithCancellationExtension
    {
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            return task.IsCompleted ? task : task.ContinueWith(
                completedTask => completedTask.GetAwaiter().GetResult(),
                token,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
