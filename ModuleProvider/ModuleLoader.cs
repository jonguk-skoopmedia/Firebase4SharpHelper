using EdgeJs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FirebaseSharp.ModuleProvider
{
    public static class ModuleLoader
    {
        private static Dictionary<string, dynamic> mProvider;

        public static async Task LoadModules(IEnumerable<ModuleConfigure> moduleConfigures)
        {
            using (EventLog log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry("Module Loader runing", EventLogEntryType.Information, 1001, 0);
            }

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

            using (EventLog log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry("Edge.func load", EventLogEntryType.Information, 1001, 0);
            }

            mProvider = new Dictionary<string, dynamic>((dynamic)await func(moduleConfigures));

            using (EventLog log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry("provider loaded", EventLogEntryType.Information, 1001, 0);
            }
        }

        public static dynamic GetProvider(string providerName)
        {
            return mProvider[providerName];
        }
    }
}
