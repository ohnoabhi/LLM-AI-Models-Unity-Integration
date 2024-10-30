
# import os
# from groq import Groq

# def get_chat_response():
#     client = Groq(
#         api_key="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",
#     )

#     chat_completion = client.chat.completions.create(
#         messages=[
#             {
#                 "role": "user",
#                 "content": "Explain the importance of fast language models",
#             }
#         ],
#         model="llama3-8b-8192",
#     )
    
#     return chat_completion.choices[0].message.content

#==========================================================================================

import json
from groq import Groq  
def get_chat_response():
    try:
        # Load configuration from JSON file
        with open('Assets/GrokUnityIntegration/config.json') as json_file:
            config = json.load(json_file)

        # Initialize Groq client
        client = Groq(api_key="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")
        
        # Create chat completion request
        chat_completion = client.chat.completions.create(
            messages=[

                {
                    "role": "system",
                    "content": "Address me as my lord when replying"
                },
                {
                    "role": config['Role'],       # Make sure 'Role' exists in config
                    "content": config['Content'],  # Make sure 'Content' exists in config
                }
            ],
            model=config['model'],
            #model="llama3-8b-8192",

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