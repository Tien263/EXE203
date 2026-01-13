"""Test Gemini API"""
import os
from dotenv import load_dotenv

load_dotenv()

try:
    import google.generativeai as genai
    
    api_key = os.getenv("GEMINI_API_KEY")
    print(f"API Key: {api_key[:20]}...")
    
    genai.configure(api_key=api_key)
    
    # List available models
    print("\n📋 Available models:")
    for m in genai.list_models():
        if 'generateContent' in m.supported_generation_methods:
            print(f"  - {m.name}")
    
    # Test with gemini-2.0-flash (same as in llm_service.py)
    print("\n🧪 Testing gemini-2.0-flash...")
    model = genai.GenerativeModel('gemini-2.0-flash')
    response = model.generate_content("Xin chào! Hãy giới thiệu ngắn gọn về bạn bằng tiếng Việt.")
    print(f"✅ Response: {response.text}")
    
except Exception as e:
    print(f"❌ Error: {e}")
