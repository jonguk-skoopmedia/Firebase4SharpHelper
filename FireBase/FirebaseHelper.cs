using FirebaseSharp.Firebase.Models;
using FirebaseSharp.ModuleProvider;
using FirebaseSharp.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace FirebaseSharp.Firebase
{
    public static class FirebaseHelper
    {
        private class FirebaseInitConfigure
        {
            public string AppSdk { get; set; }
            public string DbSdk { get; set; }
            public string AuthSdk { get; set; }
            public ReatimeDBConfigure DbConfigure { get; set; }
        }

        public static string Configure { get; set; }
        private static dynamic mFirebaseWapper;

        private static ConcurrentDictionary<string, List<EventHandler<ObserveEventArgs>>> mEventManager = new ConcurrentDictionary<string, List<EventHandler<ObserveEventArgs>>>();
        private static object locker = new object();

        public static async Task<bool> InitFirebaseAsync(string assemblyLocation, ReatimeDBConfigure configure)
        {
            if (assemblyLocation == null || assemblyLocation == string.Empty)
            {
                return false;
            }

            await ModuleLoader.LoadModules(new ModuleConfigure[] {
                 new ModuleConfigure
                {
                    ModuleName="firebase",
                    ModulePath=$"{assemblyLocation}/Firebase/firebase_proxy_module"
                },
            });

            mFirebaseWapper = ModuleProvider.ModuleLoader.GetProvider("firebase").firebaseProvider;

            var firebaseIntializer = (Func<object, Task<object>>)mFirebaseWapper.init;
            var providerIntializer = (Func<object, Task<object>>)mFirebaseWapper.initProvider;

            var firebaseConfig = new FirebaseInitConfigure
            {
                AppSdk = $"{assemblyLocation}/node_modules/firebase/app",
                DbSdk = $"{assemblyLocation}/node_modules/firebase/database",
                AuthSdk = $"{assemblyLocation}/node_modules/firebase/auth",
                DbConfigure = configure
            };

            Configure = JsonConvert.SerializeObject(firebaseConfig);

            System.Diagnostics.Debug.WriteLine(Configure);


            ProviderConfigure providerConfigure = new ProviderConfigure
            {
                ProviderPath = $"{assemblyLocation}/edge/edge",
                AssemblyPath = $"{assemblyLocation}/FirebaseSharp.dll",
                TargetName = "FirebaseSharp.Firebase.Models.ObserveProxy",
                TargetMethod = nameof(ObserveProxy.InvokeFromFirebase),
            };

            var providerInitResult = await providerIntializer(providerConfigure);
            if ((bool)providerInitResult)
            {
                var initializeResult = await firebaseIntializer(Configure);
                return (bool)initializeResult;
            }

            return true;
        }

        public static async Task<bool> InitDatabaseAsync()
        {
            var dbInitTask = (Func<object, Task<object>>)mFirebaseWapper.initDatabase;

            var initializeResult = await dbInitTask("dummy");
            ObserveProxy.InvokedFromFDBObserve += ObserveProxy_InvokedFromFDBObserve;

            return (bool)initializeResult;
        }

        public static async Task<bool> Authorize(SecureString token)
        {
            var authorizeTask = (Func<object, Task<object>>)mFirebaseWapper.auth;
            var authorizeResult = await authorizeTask(token.ToStringFromSecureString());

            if (authorizeResult is bool)
            {
                return (bool)authorizeResult;
            }

            dynamic error = authorizeResult;

            var errorCode = error.code;
            var errorMessage = error.message;

            // TODO: logging

            return false;
        }

        public static async Task<FirebaseResponse> Select(string query)
        {
            var selectTask = (Func<object, Task<object>>)mFirebaseWapper.select;
            var selectResult = await selectTask(query);

            Dictionary<string, object> body = JsonConvert.DeserializeObject<Dictionary<string, object>>(selectResult.ToString());

            return new FirebaseResponse(true, (int)200, body, selectResult.ToString());
        }

        public static async Task Observe(string path, EventHandler<ObserveEventArgs> handler)
        {
            bool isNewObserveRequrest = false;
            if (mEventManager.ContainsKey(path))
            {
                mEventManager[path].Add(handler);
            }
            else
            {
                if (mEventManager.TryAdd(path, new List<EventHandler<ObserveEventArgs>>()))
                {
                    mEventManager[path].Add(handler);

                    isNewObserveRequrest = true;
                }
            }

            if (isNewObserveRequrest)
            {
                var observeTask = (Func<object, Task<object>>)mFirebaseWapper.observe;
                var observeResult = await observeTask(path);
            }
        }

        public static async void StopObserve(string path, EventHandler<ObserveEventArgs> handler)
        {
            bool isClearObserve = false;

            if (mEventManager.ContainsKey(path))
            {
                if (mEventManager[path].Count - 1 > 0)
                {
                    mEventManager[path].Remove(handler);
                }
                else
                {
                    mEventManager.TryRemove(path, out List<EventHandler<ObserveEventArgs>> value);
                    value.Clear();

                    isClearObserve = true;
                }
            }

            if (isClearObserve)
            {
                var stopObserveTask = (Func<object, Task<object>>)mFirebaseWapper.stopObserve;
                bool result = (bool)await stopObserveTask(path);
            }
        }

        public static void DisposeAsync()
        {
            if (mEventManager.Keys.Count > 0)
            {
                var keyEnumerator = mEventManager.Keys.GetEnumerator();

                while (keyEnumerator.MoveNext())
                {
                    EventHandler<ObserveEventArgs>[] stopObserveTasks = mEventManager[keyEnumerator.Current].ToArray();
                    foreach (EventHandler<ObserveEventArgs> handler in stopObserveTasks)
                    {
                        StopObserve(keyEnumerator.Current, handler);
                    }
                }
            }
        }

        private static void ObserveProxy_InvokedFromFDBObserve(object sender, InvokeEventArgs e)
        {
            // TODO: saperate data flow
            dynamic observe = e.Result;
            dynamic path = (string)observe.path;
            string lowData = observe.value;

            foreach (EventHandler<ObserveEventArgs> handler in mEventManager[path])
            {
                handler?.BeginInvoke(null, new ObserveEventArgs(lowData, path), null, null);
            }
        }
    }
}
