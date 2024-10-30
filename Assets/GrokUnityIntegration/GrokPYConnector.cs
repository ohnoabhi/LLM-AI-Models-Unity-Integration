// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics;
// using UnityEngine;

// public class GrokPYConnector : MonoBehaviour
// {
//     // Path to the Python executable
//     public string pythonPath = @"C:\Users\admin\AppData\Local\Programs\Python\Python313\python.exe"; //Update ur path here
    
//     // Path to the Python script
//     public string scriptPath = @"/Users/abhiram/AI_Tools/Assets/Scripts/Groq/GrokConnector.py"; //update ur path here

//     void Start()
//     {
//         // Run the Python script and get the response
//         string response = RunPythonScript(pythonPath, scriptPath);
//         UnityEngine.Debug.Log(response);
//     }

//     string RunPythonScript(string pythonExePath, string scriptFilePath)
//     {
//         try
//         {
//             // Set up the process info for running the Python script
//             ProcessStartInfo start = new ProcessStartInfo
//             {
//                 FileName = pythonExePath, // Full path to python executable
//                 Arguments = scriptFilePath, // Full path to Python script
//                 UseShellExecute = false, // To capture the output
//                 RedirectStandardOutput = true, // Capture the output
//                 CreateNoWindow = true // Hide the Python window
//             };

//             using (Process process = Process.Start(start))
//             {
//                 // Read the output from the Python script
//                 using (System.IO.StreamReader reader = process.StandardOutput)
//                 {
//                     string result = reader.ReadToEnd();
//                     UnityEngine.Debug.Log(result);
//                     return result.Trim(); // Returning the result back to Unity
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             UnityEngine.Debug.LogError("Error running Python script: " + ex.Message);
//             return string.Empty;
//         }
//     }
// }


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;

public class GrokPYConnector : MonoBehaviour
{
    // Path to the Python executable
    public string pythonPath = @"C:\Users\admin\AppData\Local\Programs\Python\Python313\python.exe";
    
    // Path to the Python script
    public string scriptPath = @"/Users/abhiram/AI_Tools/Assets/Scripts/Groq/GrokConnector.py";

    void Start()
    {
        StartCoroutine(RunPythonScriptCoroutine());
    }

    IEnumerator RunPythonScriptCoroutine()
    {
        string response = RunPythonScript(pythonPath, scriptPath);
        UnityEngine.Debug.Log($"Python Script Response: {response}");
        yield return null;
    }

    string RunPythonScript(string pythonExePath, string scriptFilePath)
    {
        try
        {
            // Verify paths exist
            if (!System.IO.File.Exists(pythonExePath))
            {
                throw new Exception($"Python executable not found at: {pythonExePath}");
            }
            if (!System.IO.File.Exists(scriptFilePath))
            {
                throw new Exception($"Python script not found at: {scriptFilePath}");
            }

            // Set up the process info
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{scriptFilePath}\"", // Wrap path in quotes to handle spaces
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // Add error redirection
                CreateNoWindow = true,
                WorkingDirectory = System.IO.Path.GetDirectoryName(scriptFilePath), // Set working directory
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo = start;
                process.EnableRaisingEvents = true;

                // Attach to output events
                process.OutputDataReceived += (sender, e) => {
                    if (e.Data != null)
                    {
                        output.AppendLine(e.Data);
                        UnityEngine.Debug.Log($"Python Output: {e.Data}");
                    }
                };

                process.ErrorDataReceived += (sender, e) => {
                    if (e.Data != null)
                    {
                        error.AppendLine(e.Data);
                        UnityEngine.Debug.LogError($"Python Error: {e.Data}");
                    }
                };

                // Start the process
                process.Start();

                // Begin async reading
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to exit
                process.WaitForExit();

                // Check for errors
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script exited with code {process.ExitCode}. Error: {error}");
                }

                return output.ToString().Trim();
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error running Python script: {ex.Message}\nStack trace: {ex.StackTrace}");
            return $"ERROR: {ex.Message}";
        }
    }
}
