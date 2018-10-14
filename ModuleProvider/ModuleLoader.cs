using EdgeJs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseSharp.ModuleProvider
{
    public static class ModuleLoader
    {
        private static Dictionary<string, dynamic> mProvider;

        public static async Task InitializeNodeJsProviderAsync(List<ModuleConfigure> moduleConfigures)
        {
            var func = Edge.Func(@"return function (targetModels, callback) {
                                            var modules = {};
                                            try
                                            {
                                                var i;
                                                for (i = 0; i < targetModels.length; i++)
                                                { 
                                                    var module = targetModels[i];
                                                    var result = require(module.ModulePath);
                                                    
                                                    modules[module.ModuleName] = result;
                                                } 
                                            }
                                            catch(exception)
                                            {
                                                console.log(exception);
                                            }

                                            callback(null, modules);
                                          };");

            mProvider = new Dictionary<string, dynamic>((dynamic)await func(moduleConfigures));
        }

        public static dynamic GetProvider(string providerName)
        {
            return mProvider[providerName];
        }
    }
}
