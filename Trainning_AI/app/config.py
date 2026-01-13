import os
from dotenv import load_dotenv

# Load .env from parent directory
base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
env_file = os.path.join(base_dir, ".env")
load_dotenv(env_file)

class Settings:
    # API Keys
    OPENAI_API_KEY = os.getenv("OPENAI_API_KEY", "")
    GEMINI_API_KEY = os.getenv("GEMINI_API_KEY", "")
    
    # Server Config
    HOST = os.getenv("HOST", "0.0.0.0")
    PORT = int(os.getenv("PORT", 8000))
    
    # Database Config
    CHROMA_DB_PATH = "./chroma_db"
    COLLECTION_NAME = "moc_chau_fruits"
    
    # Model Config - Use smaller model for low memory environments
    EMBEDDING_MODEL = os.getenv("EMBEDDING_MODEL", "sentence-transformers/all-MiniLM-L6-v2")  # Lighter model: ~80MB
    LLM_MODEL = "gpt-3.5-turbo"  # Hoặc sử dụng model khác
    
    # RAG Config
    CHUNK_SIZE = 500
    CHUNK_OVERLAP = 50
    TOP_K_RESULTS = 3
    
    # Data Paths - Multiple JSON files
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    DATA_DIR = os.path.join(BASE_DIR, "..", "data")
    DATA_PATH = os.path.join(DATA_DIR, "moc_chau_fruits.json")  # Main products
    BRAND_INFO_PATH = os.path.join(DATA_DIR, "brand_info.json")
    SEASONAL_CALENDAR_PATH = os.path.join(DATA_DIR, "seasonal_calendar.json")
    STORAGE_GUIDE_PATH = os.path.join(DATA_DIR, "storage_guide.json")

settings = Settings()
