// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
// using System.Diagnostics;
// using System;

// public enum GrokModel
// {
//     Mixtral,
//     LLaMA2_OR_Meta,
//     Gemini
// }

// public class GrokEditorTool : EditorWindow
// {
//     public string pythonPath = @"C:\Users\admin\AppData\Local\Programs\Python\Python313\python.exe";
//     public string scriptPath = @"Assets/GrokUnityIntegration/config.json";
//     private string pythonOutput = "";
//     private string userInput = "";

//     private string ScriptOnlyInstrction = "Respond only as cSharp code";
//     private bool scriptOnlyReply = false;
//     private GrokModel selectedModel = GrokModel.Mixtral; // Default selection

//     [MenuItem("Tools/Groq")]
//     public static void ShowWindow()
//     {
//         GetWindow<GrokEditorTool>("Groq");
//     }

//     void OnGUI()
//     {
//         GUILayout.Label("Groq Interaction", EditorStyles.boldLabel);

//         //scriptOnlyReply = EditorGUILayout.Toggle("Script-Only Reply", scriptOnlyReply);

//         // Dropdown for selecting the Grok model
//         GUILayout.Label("Select Grok Model:", EditorStyles.boldLabel);
//         selectedModel = (GrokModel)EditorGUILayout.EnumPopup(selectedModel);

//         // Text area for user input
//         GUILayout.Label("Enter your prompt:", EditorStyles.boldLabel);
//         userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(100));

//         pythonPath = EditorGUILayout.TextField("Python Path", pythonPath);
//         scriptPath = EditorGUILayout.TextField("Python Script Path", scriptPath);

//         if (GUILayout.Button("Run Groq"))
//         {
//             UpdatePromptWithUserPreference();
//             RunPythonScriptAsync(pythonPath, scriptPath);
//         }

//         GUILayout.Label("Groq Response:", EditorStyles.boldLabel);
//         EditorGUILayout.TextArea(pythonOutput, GUILayout.Height(200));
//     }

//     void UpdatePromptWithUserPreference()
//     {
//         GrokInputJson config = new GrokInputJson { Role = "user", Content = "Hey there!" };

//         string modelID = "";

//         switch (selectedModel)
//         {
//             case GrokModel.Mixtral:
//                 modelID = "mixtral-8x7b-32768";
//                 break;
//             case GrokModel.LLaMA2_OR_Meta:
//                 modelID = "llama3-8b-8192";
//                 break;
//             case GrokModel.Gemini:
//                 modelID = "gemma2-9b-it";
//                 break;
//         }        
        
//         UnityEngine.Debug.Log(userInput.ToString() + modelID);
//         //config.UpdateConfig("user", userInput, modelID);
//         config.UpdateConfig("user", userInput, modelID);

//         //if(!scriptOnlyReply) 


//         //else config.UpdateConfig("user", userInput, modelID, ScriptOnlyInstrction);


//         // You might want to pass the selected model to your Python script here
//         // For example, by modifying the config or adding it to the command line arguments
//     }

//     async void RunPythonScriptAsync(string pythonExePath, string scriptFilePath)
//     {
//         try
//         {
//             ProcessStartInfo start = new ProcessStartInfo
//             {
//                 FileName = pythonExePath,
//                 // Include the selected model in the arguments
//                 Arguments = $"{scriptFilePath} \"{userInput}\" {selectedModel}",
//                 UseShellExecute = false,
//                 RedirectStandardOutput = true,
//                 RedirectStandardError = true,
//                 CreateNoWindow = true
//             };

//             using (Process process = Process.Start(start))
//             {
//                 string result = await process.StandardOutput.ReadToEndAsync(); 
//                 string error = await process.StandardError.ReadToEndAsync(); 

//                 if (!string.IsNullOrEmpty(error))
//                 {
//                     pythonOutput = "Python Script Error: " + error;
//                 }
//                 else
//                 {
//                     pythonOutput = result;
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             pythonOutput = "Error running Python script: " + ex.Message;
//         }

//         Repaint();
//     }
// }
// #endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;

public enum GrokModel
{
    Mixtral,
    LLaMA2_OR_Meta,
    Gemini
}

public class GrokEditorTool : EditorWindow
{
    public string pythonPath = PATHVARIABLES.PYTHON_PATH;
    public string scriptPath = PATHVARIABLES.SCRIPT_PATH;
    private string pythonOutput = "";
    private string userInput = "";
    private bool isProcessing = false;

    private string ScriptOnlyInstrction = "Respond only as cSharp code";
    private bool scriptOnlyReply = false;
    private GrokModel selectedModel = GrokModel.Mixtral;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Groq")]
    public static void ShowWindow()
    {
        GetWindow<GrokEditorTool>("Groq");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Groq Interaction", EditorStyles.boldLabel);

        GUILayout.Label("Select Grok Model:", EditorStyles.boldLabel);
        selectedModel = (GrokModel)EditorGUILayout.EnumPopup(selectedModel);

        GUILayout.Label("Enter your prompt:", EditorStyles.boldLabel);
        userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(100));

        pythonPath = EditorGUILayout.TextField("Python Path", pythonPath);
        scriptPath = EditorGUILayout.TextField("Python Script Path", scriptPath);

        GUI.enabled = !isProcessing;
        if (GUILayout.Button(isProcessing ? "Processing..." : "Run Groq"))
        {
            if (ValidatePaths())
            {
                isProcessing = true;
                UpdatePromptWithUserPreference();
                RunPythonScript(pythonPath, scriptPath);
            }
        }
        GUI.enabled = true;

        GUILayout.Label("Groq Response:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(pythonOutput, GUILayout.Height(200));

        EditorGUILayout.EndScrollView();
    }

    private bool ValidatePaths()
    {
        if (!File.Exists(pythonPath))
        {
            EditorUtility.DisplayDialog("Error", 
                "Python executable not found at specified path. Please verify the Python path.", "OK");
            return false;
        }

        if (!File.Exists(scriptPath))
        {
            EditorUtility.DisplayDialog("Error", 
                "Python script not found at specified path. Please verify the script path.", "OK");
            return false;
        }

        return true;
    }

    void UpdatePromptWithUserPreference()
    {
        string modelID = selectedModel switch
        {
            GrokModel.Mixtral => "mixtral-8x7b-32768",
            GrokModel.LLaMA2_OR_Meta => "llama3-8b-8192",
            GrokModel.Gemini => "gemma2-9b-it",
            _ => "mixtral-8x7b-32768"
        };

        UnityEngine.Debug.Log($"Processing request with model: {modelID}");
        UnityEngine.Debug.Log($"User Input: {userInput}");

        // Create config with the selected model
        GrokInputJson config = new GrokInputJson { Role = "user", Content = userInput };
        config.UpdateConfig("user", userInput, modelID);
    }

    void RunPythonScript(string pythonExePath, string scriptFilePath)
    {
        try
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{scriptFilePath}\" \"{userInput}\" {selectedModel}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(scriptFilePath),
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            UnityEngine.Debug.Log($"Executing Python script with arguments: {start.Arguments}");

            using (Process process = new Process())
            {
                process.StartInfo = start;
                process.EnableRaisingEvents = true;

                StringBuilder outputBuilder = new StringBuilder();
                StringBuilder errorBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                        UnityEngine.Debug.Log($"Python Output: {e.Data}");
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.AppendLine(e.Data);
                        UnityEngine.Debug.LogError($"Python Error: {e.Data}");
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    pythonOutput = $"Python script failed with exit code {process.ExitCode}.\nError: {errorBuilder}";
                }
                else
                {
                    pythonOutput = outputBuilder.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            pythonOutput = $"Error executing Python script: {ex.Message}\nStack trace: {ex.StackTrace}";
            UnityEngine.Debug.LogError($"Python script execution failed: {ex}");
        }
        finally
        {
            isProcessing = false;
            Repaint();
        }
    }
}
#endif