# Firebase4SharpHelper

* **Notice**
if you don't need to firebase custom token authorize more batter [FireSharp](https://github.com/ziyasal/FireSharp/)
, if you need firebase custom authorization in c# client consider using this package

* **structure**
```
 ------------------------------                ------------------------              ----------------------
|                              |              |                        |            |                      |
|         firebase sdk         |              |                        |            |                      |
|        (node js module)      |  < ----- >   |      fb interface      |   < ---- > |     edge js module   |
|                              |              |                        |            |                      |
|                              |              |                        |            |                      |
 ------------------------------                ------------------------              ----------------------
                                                                                                ▲
                                                                                                |
                                                                                                |
                                                                                                ▼
                                                                                      ----------------------
                                                                                     |                      |
                                                                                     |     helper class     |
                                                                                     |                      |
                                                                                      ----------------------
```

# How to use

* **Initialize firebase sdk [configuration reference](https://firebase.google.com/docs/database/web/start)**
```
     bool initializeResult = await FirebaseHelper.InitializeFirebaseAsync(new RealtimeDbConfigure {
                                                                              ApiKey = "{firebase api key}",
                                                                              authDomain = "{firebase projectId}",
                                                                              databaseUrl = "{db url},
                                                                              storageBucket = "{storage}
                                                                          });
                                                                     
```

* **how to Connect to Firebase realtime database**

    if succeded initialize, you can be connect database
```
     bool initializeresult = await FirebaseHelper.InitDatabaseAsync();

```
#### how to authorize firease 
```
      ///
      /// string value to secure string extension function
      ///
      public static SecureString ToSecureString(this string value)
        {
            SecureString secureString;
            unsafe
            {
                int length = value.Length;
                fixed (char* secureValue = value.ToCharArray())
                {
                    secureString = new SecureString(secureValue, length);
                }
            }

            return secureString;
        }
       
                         .
                         .
                         .
        
   // authorize
   SecureString token = "token".ToSecureString();
   bool authorizeResult = await FirebaseHelper.Authorize(token);

```

* **How to Observe firebase change**
````
   FirebaseHelper.Observe($"path", (sender, args) => {
      TODO
   });
````
* **Stop Observe**
````
   FirebaseHelper.StopObserve("path", observed eventhandler);
````

* **Select Firebase**
````
    var firebaseResponse  = FirebaseHelper.Select(queryString);

    string body = firebaseResponse.RowBody;
    Console.WriteLine(body);
````

* **How to Disconnect Firebase**
````
   FirebaseHelper.DisposeAsync();
````

# Future
* db write
* separete lisening event
