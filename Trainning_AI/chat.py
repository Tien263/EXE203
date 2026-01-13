"""
Script chat trực tiếp với AI - Chạy local
"""
import sys
import os
import warnings
warnings.filterwarnings('ignore')

# Set UTF-8 encoding for console output
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

# Add app directory to path
sys.path.insert(0, os.path.dirname(__file__))

try:
    from app.vector_store import VectorStore
    vector_store_class = VectorStore
except ImportError:
    print("⚠️ ChromaDB không khả dụng, sử dụng SimpleVectorStore")
    from app.simple_vector_store import SimpleVectorStore
    vector_store_class = SimpleVectorStore

from app.llm_service import LLMService
from app.config import settings


def main():
    print("=" * 60)
    print("🤖 MỘC CHÂU FRUITS AI CHATBOT")
    print("=" * 60)
    print("Hỏi tôi bất cứ điều gì về hoa quả Mộc Châu!")
    print("Gõ 'exit' hoặc 'quit' để thoát.\n")
    
    # Initialize services
    print("📦 Đang khởi tạo AI...")
    vector_store = vector_store_class()
    llm_service = LLMService()
    
    # Check if data is loaded
    count = vector_store.get_collection_count()
    if count == 0:
        print("❌ Chưa có dữ liệu! Vui lòng chạy 'python train.py' trước.")
        return
    
    print(f"✅ Đã load {count} loại hoa quả vào hệ thống\n")
    print("-" * 60)
    
    # Chat loop
    while True:
        try:
            # Get user input
            question = input("\n💬 Bạn: ").strip()
            
            # Check exit commands
            if question.lower() in ['exit', 'quit', 'thoát', 'bye']:
                print("\n👋 Tạm biệt! Hẹn gặp lại!")
                break
            
            # Skip empty input
            if not question:
                continue
            
            # Search for relevant information
            print("\n🔍 Đang tìm kiếm thông tin...")
            # If asking about mini pack/50g, search more to get all products
            if any(word in question.lower() for word in ['50g', 'mini', 'gói nhỏ', 'mix']):
                results = vector_store.search(question, top_k=10)  # Get all products
            else:
                results = vector_store.search(question, top_k=5)
            
            if not results:
                print("🤖 AI: Xin lỗi, tôi không tìm thấy thông tin liên quan. Hãy thử hỏi về: Dâu tây, Mận, Xoài, Đào, Hồng, Mít, Chuối, Sữa chua sấy.")
                continue
            
            # Generate response with streaming
            print("💭 Đang suy nghĩ...\n")
            print("🤖 AI: ", end='', flush=True)
            answer = llm_service.generate_response(question, results)
            
            # Print the answer
            if answer and answer.strip():
                print(answer)
            else:
                print("(Không có câu trả lời)")
            
            print()  # New line after streaming
            
            # Show sources
            print("\n📚 Nguồn tham khảo:")
            for i, result in enumerate(results[:2], 1):
                fruit_name = result['metadata'].get('fruit_name', 'Unknown')
                relevance = (1 - result['distance']) * 100
                print(f"   {i}. {fruit_name} (độ liên quan: {relevance:.1f}%)")
            
            print("-" * 60)
            
        except KeyboardInterrupt:
            print("\n\n👋 Tạm biệt! Hẹn gặp lại!")
            break
        except Exception as e:
            print(f"\n❌ Lỗi: {e}")
            print("Vui lòng thử lại!")


if __name__ == "__main__":
    main()
