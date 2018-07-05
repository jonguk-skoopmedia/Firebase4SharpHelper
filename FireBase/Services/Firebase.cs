
using FirebaseSharp.FireBase.Enums;
using FirebaseSharp.FireBase.Error;
using FirebaseSharp.FireBase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseSharp.FireBase
{
    public class Firebase
    {
        public static readonly string FIREBASE_API_JSON_EXTENSION = ".json";

        private string mBaseUrl;
        public string BaseUrl
        {
            get
            {
                return mBaseUrl;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {

                    string msg = "baseUrl cannot be null or empty; was: '" + mBaseUrl + "'";

                    // LOGGER.error(msg );
                    throw new FirebaseException(msg);
                }

                mBaseUrl = value;
            }
        }

        private string mSecureToken = null;
        public string SecureToken
        {
            get
            {
                return mSecureToken;
            }
            set
            {
                mSecureToken = value;
            }
        }

        public async Task<FirebaseResponse> Get()
        {
            return await Get();
        }


        /// <summary>
        /// GETs data from the provided-path relative to the base-url.
        /// param path -- if null/empty, refers to the base-url
        /// @return
        /// {@link FirebaseResponse }
        /// @throws UnsupportedEncodingException 
        /// @throws {@link FirebaseException}
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<FirebaseResponse> Get(string path = null, Dictionary<string, string> query = null)
        {
            string url = BuildFullUrlFromRelativePath(path, query);

            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);

                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.GET, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                // TODO: loging
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Patch(Dictionary<string, object> data)
        {
            return await Patch(null, data);
        }

        public async Task<FirebaseResponse> Patch(string path, Dictionary<string, object> data, Dictionary<string, string> query = null)
        {
            string url = BuildFullUrlFromRelativePath(path, query);

            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                request.Headers = BuildEntityFromDictionary(data);

                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.PATCH, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Patch(string path, string jsonData, Dictionary<string, string> query = null)
        {
            string url = BuildFullUrlFromRelativePath(path, query);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers = BuildEntityFromJsonData(jsonData);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.PATCH, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Put(string path, Dictionary<string, object> data)
        {
            string url = BuildFullUrlFromRelativePath(path);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers = BuildEntityFromDictionary(data);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.PUT, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Put(string path, string jsonData)
        {
            string url = BuildFullUrlFromRelativePath(path);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers = BuildEntityFromJsonData(jsonData);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.PUT, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Post(string path, Dictionary<string, object> data)
        {
            string url = BuildFullUrlFromRelativePath(path);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers = BuildEntityFromDictionary(data);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.POST, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Post(string path, string jsonData)
        {
            string url = BuildFullUrlFromRelativePath(path);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers = BuildEntityFromJsonData(jsonData);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.POST, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

        public async Task<FirebaseResponse> Delete(string path)
        {
            string url = BuildFullUrlFromRelativePath(path);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);

            try
            {
                FirebaseResponse firebaseResponse = null;
                using (HttpWebResponse response = await GetResponse(request))
                {
                    firebaseResponse = await ProcessResponse(EFirebaseRestMethod.DELETE, response);
                }

                return firebaseResponse;
            }
            catch (Exception exception)
            {
                throw new FirebaseException(exception.Message, exception);
            }
        }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async Task<FirebaseDbListener> ListeningAsync(string path, Dictionary<string, string> query = null)
#pragma warning restore CS1998
        {
            Uri uri = new Uri(BuildFullUrlFromRelativePath(path, query));

            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(uri);

            webRequest.Accept = "text/event-stream";
            
            return new FirebaseDbListener(webRequest);
        }


        #region private methods
        private string BuildFullUrlFromRelativePath(string path, IEnumerable<KeyValuePair<string, string>> qeury = null)
        {
            // massage the path (whether it's null, empty, or not) into a full URL
            if (path == null)
            {
                path = "";
            }

            path = path.Trim();

            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }

            if (string.IsNullOrEmpty(mBaseUrl.Trim()))
            {
                string msg = "baseUrl cannot be null or empty; was: '" + mBaseUrl + "'";

                // LOGGER.error(msg );
                throw new FirebaseException(msg);
            }

            string url = mBaseUrl + path + Firebase.FIREBASE_API_JSON_EXTENSION;

            if (qeury != null)
            {
                url += "?";

                IEnumerator<KeyValuePair<string, string>> enumerator = qeury.GetEnumerator();

                KeyValuePair<string, string> e;

                while (enumerator.MoveNext())
                {
                    try
                    {
                        e = enumerator.Current;
                        url += e.Key + "=" + e.Value + "&";
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }

            if (mSecureToken != null)
            {
                if (qeury != null)
                {
                    url += "auth=" + mSecureToken;
                }
                else
                {
                    url += "?auth=" + mSecureToken;
                }

            }

            if (url.LastIndexOf("&") == url.Length)
            {

                StringBuilder str = new StringBuilder(url);
                str.Remove(str.Length - 1, 1);

                url = str.ToString();
            }

            // LOGGER.info("built full url to '" + url + "' using relative-path of '" + path + "'");
            return url;
        }


        private static WebHeaderCollection BuildEntityFromJsonData(string jsonData)
        {
            WebHeaderCollection result = null;

            try
            {
                result = new WebHeaderCollection();
                Dictionary<string, string> headers = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                foreach (var keyPaire in headers)
                {
                    result.Add(keyPaire.Key, keyPaire.Value);
                }
            }
            catch (Newtonsoft.Json.JsonException parseException)
            {
                string msg = "unable to create entity from data; data was: " + jsonData;
                // LOGGER.error(msg );
                throw new FirebaseException(msg, parseException);
            }

            return result;
        }

        private static WebHeaderCollection BuildEntityFromDictionary(Dictionary<string, object> data)
        {
            return BuildEntityFromJsonData(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        private static async Task<HttpWebResponse> GetResponse(HttpWebRequest request)
        {
            if (request == null)
            {
                string msg = "request cannot be null";

                // LOGGER.error(msg);
                throw new FirebaseException(msg);
            }

            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)await request.GetResponseAsync();

                return webResponse;
            }
            catch (Exception t)
            {
                string msg = "unable to receive response from request(" + request.Method + ") @ " + request.RequestUri;

                // LOGGER.error(msg);

                throw new FirebaseException(msg, t);
            }
        }

        private static async Task<FirebaseResponse> ProcessResponse(EFirebaseRestMethod method, HttpWebResponse httpResponse)
        {
            FirebaseResponse response = null;

            // sanity-checks

            if (method == EFirebaseRestMethod.NONE)
            {
                string msg = "method cannot be null";

                //LOGGER.error(msg );
                throw new FirebaseException(msg);
            }

            if (httpResponse == null)
            {
                String msg = "httpResponse cannot be null";

                // LOGGER.error(msg);
                throw new FirebaseException(msg);
            }

            bool success = false;

            switch (method)
            {

                case EFirebaseRestMethod.DELETE:

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent
                        && "No Content".Equals(httpResponse.StatusDescription, StringComparison.CurrentCultureIgnoreCase))
                    {
                        success = true;
                    }
                    break;

                case EFirebaseRestMethod.PATCH:
                case EFirebaseRestMethod.PUT:
                case EFirebaseRestMethod.POST:
                case EFirebaseRestMethod.GET:

                    if (httpResponse.StatusCode == HttpStatusCode.OK
                        && "OK".Equals(httpResponse.StatusDescription, StringComparison.CurrentCultureIgnoreCase))
                    {
                        success = true;
                    }
                    break;

                default:
                    break;
            }

            // get the response-body
            if (!success)
            {
                goto Failed;
            }

            if (httpResponse.ContentLength > 0)
            {
                try
                {
                    int maxCapacity = 0;
                    if (httpResponse.ContentLength > int.MaxValue)
                    {
                        maxCapacity = int.MaxValue;
                    }
                    else
                    {
                        maxCapacity = (int)httpResponse.ContentLength;
                    }

                    StringBuilder builder = new StringBuilder(maxCapacity);
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        builder.Append(await streamReader.ReadToEndAsync());
                    }

                    Dictionary<string, object> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(builder.ToString());
                    response = new FirebaseResponse(success, (int)httpResponse.StatusCode, body, builder.ToString());

                    builder.Clear();

                    return response;
                }
                catch (IOException ioException)
                {
                    // LOGGER.error(msg);
                    throw new FirebaseException(ioException.Message, ioException);
                }
                catch (Newtonsoft.Json.JsonReaderException jsonParseException)
                {
                    throw new FirebaseException(jsonParseException.Message, jsonParseException);
                }
            }

            Failed:
            return new FirebaseResponse(success, (int)httpResponse.StatusCode, null, null);
        }
        #endregion

    }
}
