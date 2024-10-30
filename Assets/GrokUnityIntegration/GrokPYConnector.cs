using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GrokPYConnector : MonoBehaviour
{
    // Path to the Python executable
    public string pythonPath = @"/Users/abhiram/AI_Tools/Library/PythonInstall/bin/python3";
    
    // Path to the Python script
    public string scriptPath = @"/Users/abhiram/AI_Tools/Assets/Scripts/Groq/GrokConnector.py";

    void Start()
    {
        // Run the Python script and get the response
        string response = RunPythonScript(pythonPath, scriptPath);
        UnityEngine.Debug.Log(response);
    }

    string RunPythonScript(string pythonExePath, string scriptFilePath)
    {
        try
        {
            // Set up the process info for running the Python script
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonExePath, // Full path to python executable
                Arguments = scriptFilePath, // Full path to Python script
                UseShellExecute = false, // To capture the output
                RedirectStandardOutput = true, // Capture the output
                CreateNoWindow = true // Hide the Python window
            };

            using (Process process = Process.Start(start))
            {
                // Read the output from the Python script
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result.Trim(); // Returning the result back to Unity
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error running Python script: " + ex.Message);
            return string.Empty;
        }
    }
}
