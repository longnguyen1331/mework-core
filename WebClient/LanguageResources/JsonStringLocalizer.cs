using Core.Const;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace WebClient.LanguageResources
{
    public class JsonStringLocalizer
    {
        public readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        public string Code { get; set; } = CultureType.VI;
        private List<LocalizedString> LocalizedString = new();

        public JsonStringLocalizer()
        {
            LoadResources(Code);
        }
    
        public void LoadResources(string code)
        {
            LocalizedString = new();
            string filePath = $@"LanguageResources/App.{code}.json";
            using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(str))
            using (var reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        continue;
                    string key = (string) reader.Value;
                    reader.Read();
                    string value = _jsonSerializer.Deserialize<string>(reader);
                    LocalizedString.Add(new LocalizedString(key, value, false));
                }
            }   
        }
    
    
        public LocalizedString this[string name]
        {
            get
            {
                var value =  LocalizedString.FirstOrDefault(x => x.Name == name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                    : actualValue;
            }
        }
        
        
        public LocalizedString this[string name,bool isApiCode]
        {
            get
            {
                string output = Regex.Replace(name,@"\[(.*?)\]", match =>
                {
                    return this[match.Groups[1].Value];
                });
                return new LocalizedString(output, output ?? name, output == null);
            }
        }

        private ApiCodeString GetApiCode(string input)
        {
            string pattern = @"";
            
            var matches = Regex.Matches(input, pattern);

            var apiCode = new ApiCodeString();
            if (matches.Count > 0)
            {
                var first = true;
                foreach (Match  match in matches )
                {
                    if (first)
                    {
                        apiCode.Code = match.Groups[1].Value ;
                        first = false;
                        continue;
                    }
                    apiCode.Values.AddLast(match.Groups[1].Value);
                }
                
                return apiCode;
            }

            return new ApiCodeString();
        }
    }

    public class ApiCodeString
    {
        public string Code { get; set; }
        public List<string> Values { get; set; } = new();
    }
}