#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;

public enum GrokModel
{
    Mixtral,
    LLaMA2_OR_Meta,
    Gemini
}

public class GrokEditorTool : EditorWindow
{
    public string pythonPath = @"C:\Users\admin\AppData\Local\Programs\Python\Python313\python.exe";
    public string scriptPath = @"Assets/GrokUnityIntegration/config.json";
    private string pythonOutput = "";
    private string userInput = "";

    private string ScriptOnlyInstrction = "Respond only as cSharp code";
    private bool scriptOnlyReply = false;
    private GrokModel selectedModel = GrokModel.Mixtral; // Default selection

    [MenuItem("Tools/Groq")]
    public static void ShowWindow()
    {
        GetWindow<GrokEditorTool>("Groq");
    }

    void OnGUI()
    {
        GUILayout.Label("Groq Interaction", EditorStyles.boldLabel);

        //scriptOnlyReply = EditorGUILayout.Toggle("Script-Only Reply", scriptOnlyReply);

        // Dropdown for selecting the Grok model
        GUILayout.Label("Select Grok Model:", EditorStyles.boldLabel);
        selectedModel = (GrokModel)EditorGUILayout.EnumPopup(selectedModel);

        // Text area for user input
        GUILayout.Label("Enter your prompt:", EditorStyles.boldLabel);
        userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(100));

        pythonPath = EditorGUILayout.TextField("Python Path", pythonPath);
        scriptPath = EditorGUILayout.TextField("Python Script Path", scriptPath);

        if (GUILayout.Button("Run Groq"))
        {
            UpdatePromptWithUserPreference();
            RunPythonScriptAsync(pythonPath, scriptPath);
        }

        GUILayout.Label("Groq Response:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(pythonOutput, GUILayout.Height(200));
    }

    void UpdatePromptWithUserPreference()
    {
        GrokInputJson config = new GrokInputJson { Role = "user", Content = "Hey there!" };

        string modelID = "";

        switch (selectedModel)
        {
            case GrokModel.Mixtral:
                modelID = "mixtral-8x7b-32768";
                break;
            case GrokModel.LLaMA2_OR_Meta:
                modelID = "llama3-8b-8192";
                break;
            case GrokModel.Gemini:
                modelID = "gemma2-9b-it";
                break;
        }        
        
        UnityEngine.Debug.Log(userInput.ToString() + modelID);
        //config.UpdateConfig("user", userInput, modelID);
        config.UpdateConfig("user", userInput, modelID);

        //if(!scriptOnlyReply) 


        //else config.UpdateConfig("user", userInput, modelID, ScriptOnlyInstrction);


        // You might want to pass the selected model to your Python script here
        // For example, by modifying the config or adding it to the command line arguments
    }

    async void RunPythonScriptAsync(string pythonExePath, string scriptFilePath)
    {
        try
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonExePath,
                // Include the selected model in the arguments
                Arguments = $"{scriptFilePath} \"{userInput}\" {selectedModel}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                string result = await process.StandardOutput.ReadToEndAsync(); 
                string error = await process.StandardError.ReadToEndAsync(); 

                if (!string.IsNullOrEmpty(error))
                {
                    pythonOutput = "Python Script Error: " + error;
                }
                else
                {
                    pythonOutput = result;
                }
            }
        }
        catch (Exception ex)
        {
            pythonOutput = "Error running Python script: " + ex.Message;
        }

        Repaint();
    }
}
#endif