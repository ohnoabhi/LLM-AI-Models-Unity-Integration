import json
from groq import Groq  
def get_chat_response():
    try:
        # Load configuration from JSON file
        with open(r'D:\UnityProjects\AI-Integrations_Git\LLM-AI-Models-Unity-Integration\AI-Integrations\Assets\GrokUnityIntegration\config.json') as json_file:
            config = json.load(json_file)

        # Initialize Groq client
        

        client = Groq(api_key="---UR API KEY HERE ---") #TODO:add APIKEY from Groq here
        
        # Create chat completion request
        chat_completion = client.chat.completions.create(
            messages=[

                {
                    "role": "system",
                    "content": "Address me as my lord when replying" # TODO : Update here with ur personalisations for the LLM
                },
                {
                    "role": config['Role'],       # Make sure 'Role' exists in config
                    "content": config['Content'],  # Make sure 'Content' exists in config
                }
            ],
            model=config['model'],

        )
        
        # Return the response content
        return chat_completion.choices[0].message.content

    except FileNotFoundError:
        return "Error: Configuration file not found."
    except json.JSONDecodeError:
        return "Error: Configuration file is not valid JSON."
    except Exception as ex:
        return f"An error occurred: {str(ex)}"

if __name__ == "__main__":
    response = get_chat_response()
    print(response)