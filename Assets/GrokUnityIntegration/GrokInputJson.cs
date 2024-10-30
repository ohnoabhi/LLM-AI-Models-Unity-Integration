using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

public class GrokInputJson 
{
    public string Role { get; set; }
    public string Content { get; set; }

    public string model {get; set;}

    public string systemInstruction { get; set; }

    public void UpdateConfig(string role, string content, string model, string systemInstruction = null)
    {   
        GrokInputJson config;

        if(systemInstruction == null)
            config = new GrokInputJson { Role = role, Content = content, model = model, systemInstruction = ""};
        else
            config = new GrokInputJson { Role = role, Content = content, model = model, systemInstruction = systemInstruction};

        
        UnityEngine.Debug.Log(JsonConvert.SerializeObject(config));
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText("Assets/GrokUnityIntegration/config.json", json);
    }
}


