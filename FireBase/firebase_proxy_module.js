module.exports.firebaseProvider = {};
module.exports.firebaseProvider.initProvider = function (providerConfig, callback) {
    try {
        module.exports.firebaseProvider.provider = require(providerConfig.ProviderPath);
        module.exports.firebaseProvider.invoker = module.exports.firebaseProvider.provider.func({
            assemblyFile: providerConfig.AssemblyPath,
            typeName: providerConfig.TargetName,
            methodName: providerConfig.TargetMethod
        });
    }
    catch (exception) {
        console.log(exception);
        callback(exception, false);
    }

    callback(null, true);
};



module.exports.firebaseProvider.test = function () {
    console.log('testing');
};

module.exports.firebaseProvider.init = function (config, callback) {
    try {
        module.exports.firebaseProvider.firebase = require("../node_modules/firebase/app");
        require("../node_modules/firebase/database");
        require("../node_modules/firebase/auth");

        module.exports.firebaseProvider.firebase.initializeApp(JSON.parse(config));
    }
    catch (exception) {
        console.log(exception);

        callback(exception, false);
    }

    callback(null, true);
};

module.exports.firebaseProvider.initDatabase = function (data, callback) {
    try {
        module.exports.firebaseProvider.database = module.exports.firebaseProvider.firebase.database();
        module.exports.firebaseProvider.listeners = {};

        console.log(exports.database);
    }
    catch (exception) {
        callback(exception, false);
    }

    callback(null, true);
};
// if return true then it is succeded login or already logined
module.exports.firebaseProvider.auth = function (token, callback) {
    try {
        console.log(token);
        var user = module.exports.firebaseProvider.firebase.auth().currentUser;

        if (user === null) {
            module.exports.firebaseProvider.firebase.auth().signInWithCustomToken(token).catch(function (error) {
                console.log(error);
                callback(null, error);
            });
        }
        else {
            callback(null, true);
        }
    }
    catch (exception) {
        console.log(exception);
        callback(exception, 'orccurred unknown error when try to firebase authorization');
    }
};

module.exports.firebaseProvider.signout = function (data, callback) {
    try {
        module.exports.firebaseProvider.auth().signOut().then(function () {
            callback(null, true);
        }).catch(function (error) {
            callback(null, error);
        });
    }
    catch (exception) {
        callback(exception, null);
    }
};

module.exports.firebaseProvider.select = function (query, callback) {
    try {
        module.exports.firebaseProvider.database.ref(query).once('value').then(function (snapshot) {
            callback(null, JSON.stringify(snapshot.val()));
        });
    }
    catch (exception) {
        console.log(exception);

        callback(exception, false);
    }
};

module.exports.firebaseProvider.observe = function (path, callback) {
    try {
        module.exports.firebaseProvider.listeners[path] = module.exports.firebaseProvider.database.ref(path);
        module.exports.firebaseProvider.listeners[path].on('value', function (snapshot) {
            var paths = snapshot.ref.path.pieces_;
            var maxLength = paths.length;

            var relativePath = "";

            for (var index = 0; index < maxLength; index++) {
                relativePath += paths[index];

                if (index < maxLength - 1) {
                    relativePath += '/';
                }
            }

            var result = {
                path: relativePath,
                value: JSON.stringify(snapshot.val())
            };

            module.exports.firebaseProvider.invoker(result, function (error, result) {
            });
        });

        console.log(module.exports.firebaseProvider.listeners);

    } catch (exception) {
        callback(exception, false);
    }

    callback(null, false);
};

module.exports.firebaseProvider.stopObserve = function (path, callback) {
    try {
        module.exports.firebaseProvider.listeners[path].off();
        delete module.exports.firebaseProvider.listeners[path];
    }
    catch (exception) {
        callback(exception, false);
    }

    callback(null, true);
};