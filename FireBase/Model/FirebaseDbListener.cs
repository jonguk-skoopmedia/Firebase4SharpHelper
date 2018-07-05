using FirebaseSharp.FireBase.Enums;
using FirebaseSharp.FireBase.Extensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseSharp.FireBase.Model
{
    public class FirebaseDbListener : IDisposable
    {
        private HttpWebRequest mRequest;
        private bool mRunListening = false;
        private readonly object locker = new object();

        private CancellationTokenSource cancellation = new CancellationTokenSource();
        private Task Listener = null;

        public FirebaseDbListener(HttpWebRequest request)
        {
            mRequest = request;

            mRunListening = true;
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            Listener = Task.Run(() => EventListenerAsync(request), cancellation.Token);
#pragma warning restore CS4014
        }

        protected async void EventListenerAsync(HttpWebRequest webRequest)
        {
            WebResponse response = null;
            StreamReader stream = null;

            try
            {
                response = webRequest.GetResponse();
                stream = new StreamReader(response.GetResponseStream());

                EFirebaseRestMethod type = EFirebaseRestMethod.NONE;

                while (true)
                {
                    lock (locker)
                    {
                        if (!mRunListening)
                        {
                            break;
                        }
                    }

                    string value = await stream.ReadLineAsync().WithCancellation(cancellation.Token);

                    if (value != "")
                    {
                        cancellation.Token.ThrowIfCancellationRequested();

                        if (value.Contains("event"))
                        {
                            cancellation.Token.ThrowIfCancellationRequested();
                            if (value.Equals("event: put"))
                            {
                                type = EFirebaseRestMethod.PUT;
                            }
                            else if (value.Equals("event: patch"))
                            {
                                type = EFirebaseRestMethod.PATCH;
                            }

                        }
                        else
                        {
                            cancellation.Token.ThrowIfCancellationRequested();
                            if (value.Contains("data"))
                            {
                                // Console.WriteLine(value);
                                string subString = value.Substring(6);
                                
                                ListeningResponse result = new ListeningResponse
                                {
                                    Type = type,
                                    Data = subString
                                };

                                switch (result.Type)
                                {
                                    case EFirebaseRestMethod.PATCH:
                                        OnChanged(result);
                                        break;

                                    case EFirebaseRestMethod.PUT:
                                        OnAdded(result);
                                        break;
                                }

                                type = EFirebaseRestMethod.NONE;
                            }
                            cancellation.Token.ThrowIfCancellationRequested();
                        }
                    }
                    // Console.WriteLine(value);
                }
            }
#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.
            catch (TaskCanceledException cacneledException)
#pragma warning restore CS0168
            {

            }
            catch (Exception exception)
            {
                // TODO: loging
                Console.WriteLine(exception);
            }
            finally
            {
                if(stream != null)
                {
                    stream.Close();
                }

                if(response != null)
                {
                    response.Close();
                }

                GC.Collect();
            }
        }

        public void Dispose()
        {
            lock (locker)
            {
                if (mRunListening == true)
                {
                    mRunListening = false;
                }

                cancellation.Cancel();
            }
        }

#pragma warning disable CS0067 // Event can be possible to not using.
        public event EventHandler<ListeningResponse> DataAdded;
        public event EventHandler<ListeningResponse> DataChanged;
        public event EventHandler DataRemoved;
#pragma warning disable CS0067

        private void OnAdded(ListeningResponse value)
        {
            try
            {
                DataAdded?.Invoke(this, value);
            }
            catch (Exception e)
            {
                // TODO: logging
                Console.WriteLine(e);
            }
            
        }

        private void OnChanged(ListeningResponse value)
        {
            try
            {
                DataChanged?.Invoke(this, value);
            }
            catch(Exception e)
            {
                // TODO: Logging
                Console.WriteLine(e);
            }
        }
    }
}
