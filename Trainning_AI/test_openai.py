import os
from dotenv import load_dotenv

# Load .env file
load_dotenv()

# Check environment variables
openai_key = os.getenv("OPENAI_API_KEY", "")
print(f"OPENAI_API_KEY loaded: {openai_key[:20]}..." if openai_key else "No OPENAI_API_KEY found")

# Test OpenAI import
try:
    from openai import OpenAI
    print("✅ OpenAI library imported successfully")
    
    if openai_key:
        client = OpenAI(api_key=openai_key)
        print("✅ OpenAI client created successfully")
        
        # Test a simple API call
        try:
            response = client.chat.completions.create(
                model="gpt-4o-mini",
                messages=[{"role": "user", "content": "Hello"}],
                max_tokens=10
            )
            print("✅ OpenAI API test successful!")
            print(f"Response: {response.choices[0].message.content}")
        except Exception as e:
            print(f"❌ OpenAI API test failed: {e}")
    else:
        print("❌ No API key to test")
        
except ImportError as e:
    print(f"❌ OpenAI import failed: {e}")
except Exception as e:
    print(f"❌ OpenAI error: {e}")
