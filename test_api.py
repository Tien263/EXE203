import requests
import json

# Test API endpoint
url = "http://localhost:8000/api/chat"
data = {
    "question": "Hello test",
    "top_k": 3,
    "conversation_history": []
}

try:
    print("Testing ChatGPT API...")
    response = requests.post(url, json=data)
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.text}")
    
    if response.status_code == 200:
        result = response.json()
        print(f"Success! AI Response: {result.get('response', 'No response field')}")
    else:
        print(f"Error: {response.status_code} - {response.text}")
        
except Exception as e:
    print(f"Request failed: {e}")
