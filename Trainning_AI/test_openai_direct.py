import os
from openai import OpenAI

# Set API key directly
os.environ["OPENAI_API_KEY"] = "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"

try:
    print("Testing OpenAI directly...")
    client = OpenAI()
    
    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": "You are a helpful assistant."},
            {"role": "user", "content": "Hello"}
        ],
        max_tokens=50
    )
    
    print("Success!")
    print(f"Response: {response.choices[0].message.content}")
    
except Exception as e:
    print(f"Error: {e}")
    import traceback
    traceback.print_exc()
