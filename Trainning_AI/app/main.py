from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse
from pydantic import BaseModel
from typing import Optional, List
import json
import os
import sys

# Set UTF-8 encoding for console output
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

try:
    from app.vector_store import VectorStore
    vector_store_class = VectorStore
except ImportError:
    print("WARNING: ChromaDB khong kha dung, su dung SimpleVectorStore")
    from app.simple_vector_store import SimpleVectorStore
    vector_store_class = SimpleVectorStore

from app.llm_service import LLMService
from app.config import settings

# Initialize FastAPI app
app = FastAPI(
    title="Mộc Châu Fruits AI API",
    description="RESTful API cho AI chatbot được training với dữ liệu tùy chỉnh về hoa quả Mộc Châu. Sử dụng RAG (Retrieval-Augmented Generation) để trả lời câu hỏi.",
    version="1.0.0",
    docs_url="/docs",
    redoc_url="/redoc"
)

# CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Initialize services
vector_store = vector_store_class()
llm_service = LLMService()

# Mount static files
static_path = os.path.join(os.path.dirname(os.path.dirname(__file__)), "static")
if os.path.exists(static_path):
    app.mount("/static", StaticFiles(directory=static_path), name="static")

# Pydantic models
class Message(BaseModel):
    role: str  # 'user' or 'assistant'
    content: str

class QueryRequest(BaseModel):
    question: str
    top_k: Optional[int] = 3
    conversation_history: Optional[List[Message]] = []

class QueryResponse(BaseModel):
    answer: str
    sources: List[dict]
    action: Optional[dict] = None  # For order actions

class OrderAction(BaseModel):
    type: str  # 'add_to_cart', 'checkout'
    products: List[dict]  # [{'name': 'Dâu tây sấy dẻo', 'quantity': 2}]

class FruitData(BaseModel):
    id: str
    fruit_name: str
    description: str
    nutrients: dict
    health_benefits: List[str]
    season: str
    usage: str

class StatusResponse(BaseModel):
    status: str
    documents_count: int
    message: str


@app.on_event("startup")
async def startup_event():
    """Load dữ liệu khi khởi động server"""
    print("[STARTUP] Starting to load data...")
    print(f"[STARTUP] DATA_PATH: {settings.DATA_PATH}")
    print(f"[STARTUP] File exists: {os.path.exists(settings.DATA_PATH)}")
    
    try:
        count = vector_store.load_data_from_json()
        print(f"[OK] Da load {count} documents vao vector store")
        
        if count == 0:
            print("[WARNING] Khong co document nao duoc load!")
    except Exception as e:
        print(f"[ERROR] Loi khi load du lieu: {e}")
        import traceback
        traceback.print_exc()


@app.get("/")
async def root():
    """Serve the chat interface"""
    index_path = os.path.join(static_path, "index.html")
    if os.path.exists(index_path):
        return FileResponse(index_path)
    else:
        return {
            "name": "Mộc Châu Fruits AI API",
            "version": "1.0.0",
            "description": "API cho AI chatbot về hoa quả Mộc Châu",
            "endpoints": {
                "docs": "/docs",
                "health": "/api/health",
                "chat": "POST /api/chat",
                "fruits": "/api/fruits",
                "add_data": "POST /api/train/add",
                "reload_data": "POST /api/train/reload"
            }
        }


@app.get("/api/health")
async def health_check():
    """Health check endpoint"""
    return {
        "status": "healthy",
        "documents_count": vector_store.get_collection_count()
    }


@app.post("/api/chat", response_model=QueryResponse)
async def chat(request: QueryRequest):
    """
    Endpoint chính để chat với AI
    """
    try:
        # Debug: Print conversation history
        print(f"\n[DEBUG] Question: {request.question}")
        print(f"[DEBUG] History length: {len(request.conversation_history) if request.conversation_history else 0}")
        if request.conversation_history:
            for i, msg in enumerate(request.conversation_history[-3:]):
                print(f"  [{i}] {msg.role}: {msg.content[:50]}...")
        
        # Tìm kiếm thông tin liên quan
        search_results = vector_store.search(request.question, request.top_k)
        
        if not search_results:
            return QueryResponse(
                answer="Xin lỗi, tôi không tìm thấy thông tin liên quan. Vui lòng hỏi về các loại hoa quả Mộc Châu.",
                sources=[]
            )
        
        # Detect purchase intent
        purchase_intent = llm_service.detect_purchase_intent(request.question)
        
        # Tạo câu trả lời bằng LLM với conversation history
        answer = llm_service.generate_response(
            request.question, 
            search_results,
            conversation_history=request.conversation_history,
            purchase_intent=purchase_intent
        )
        
        # Format sources
        sources = [
            {
                "fruit_name": result['metadata'].get('fruit_name', 'N/A'),
                "relevance_score": 1 - result['distance'] if result['distance'] else None
            }
            for result in search_results
        ]
        
        # Prepare action if purchase intent detected
        action = None
        if purchase_intent and purchase_intent.get('is_purchase'):
            action = {
                'type': 'add_to_cart',
                'products': purchase_intent.get('products', [])
            }
        
        return QueryResponse(
            answer=answer,
            sources=sources,
            action=action
        )
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Lỗi xử lý: {str(e)}")


@app.post("/api/train/reload")
async def reload_data():
    """Reload dữ liệu từ file JSON"""
    try:
        if not os.path.exists(settings.DATA_PATH):
            raise HTTPException(status_code=404, detail="File dữ liệu không tồn tại")
        
        count = vector_store.load_data_from_json()
        
        return StatusResponse(
            status="success",
            documents_count=count,
            message=f"Đã reload {count} documents thành công"
        )
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Lỗi reload dữ liệu: {str(e)}")


@app.post("/api/train/add")
async def add_fruit_data(fruit: FruitData):
    """Thêm dữ liệu hoa quả mới"""
    try:
        # Thêm vào vector store
        vector_store.add_custom_data(fruit.dict())
        
        # Cập nhật file JSON
        if os.path.exists(settings.DATA_PATH):
            with open(settings.DATA_PATH, 'r', encoding='utf-8') as f:
                data = json.load(f)
        else:
            data = []
        
        data.append(fruit.dict())
        
        with open(settings.DATA_PATH, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        return {
            "status": "success",
            "message": f"Đã thêm thông tin về {fruit.fruit_name}"
        }
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Lỗi thêm dữ liệu: {str(e)}")


@app.get("/api/fruits")
async def get_all_fruits():
    """Lấy danh sách tất cả hoa quả"""
    try:
        if not os.path.exists(settings.DATA_PATH):
            return []
        
        with open(settings.DATA_PATH, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        return data
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Lỗi lấy dữ liệu: {str(e)}")


if __name__ == "__main__":
    import uvicorn
    import os
    
    # Load data before starting server
    print("\n" + "="*50)
    print("LOADING DATA...")
    print("="*50)
    print(f"DATA_PATH: {settings.DATA_PATH}")
    print(f"File exists: {os.path.exists(settings.DATA_PATH)}")
    
    try:
        count = vector_store.load_data_from_json()
        print(f"✓ Loaded {count} documents successfully!")
    except Exception as e:
        print(f"✗ Error loading data: {e}")
        import traceback
        traceback.print_exc()
    
    print("="*50 + "\n")
    
    # Support cloud deployment with PORT env variable
    port = int(os.getenv("PORT", settings.PORT))
    host = os.getenv("HOST", settings.HOST)
    
    uvicorn.run(
        app, 
        host=host, 
        port=port,
        reload=False  # Disable reload in production
    )
