import os
import uvicorn

# Set environment variable
os.environ["OPENAI_API_KEY"] = "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"

print("Starting ChatGPT AI Service...")
print(f"OpenAI API Key: {os.environ['OPENAI_API_KEY'][:20]}...")

if __name__ == "__main__":
    uvicorn.run("app.main:app", host="0.0.0.0", port=8000, reload=False)
