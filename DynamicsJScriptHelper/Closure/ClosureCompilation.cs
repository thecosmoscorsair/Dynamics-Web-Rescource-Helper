namespace DynamicsJScriptHelper.Closure
{
    using DynamicsJScriptHelper.Common;
    using DynamicsJScriptHelper.JsonModels;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using static DynamicsJScriptHelper.Common.Logger;

    internal class ClosureCompilation
    {
        private readonly string
            _apiUrl = "https://closure-compiler.appspot.com/compile",
            _compilationLevel = "SIMPLE_OPTIMIZATIONS",
            _outputInfo = "compiled_code",
            _outputFormat = "json";

        internal ClosureResponseModel ClosureResponse { get; private set; }

        internal bool CompilationComplete { get; private set; }

        private string _rawJs;

        internal void Compile()
        {
            try
            {
                using (var req = new HttpClient())
                {
                    var encodedBody = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "js_code", _rawJs },
                        { "compilation_level", _compilationLevel },
                        { "output_info", _outputInfo },
                        { "output_format", _outputFormat }
                    });

                    var resp = req.PostAsync(_apiUrl, encodedBody).Result;

                    var contents = resp.Content.ReadAsStringAsync().Result;

                    ClosureResponse = JsonConvert.DeserializeObject<ClosureResponseModel>(contents);

                    CompilationComplete = true;
                }
            }
            catch(AggregateException)
            {
                Log("Unable to contact the Closure API - Check internet connection.", true);
            }
            
        }

        internal ClosureCompilation(string rawJs)
        {
            _rawJs = rawJs;
            CompilationComplete = false;
        }
    }

    internal class ApiNotCalledException : Exception
    {
    }
}
