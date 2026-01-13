"""
Script để training/import dữ liệu vào vector store
"""
import sys
import os

# Add app directory to path
sys.path.insert(0, os.path.dirname(__file__))

try:
    from app.vector_store import VectorStore
    vector_store_class = VectorStore
except ImportError:
    print("⚠️ ChromaDB không khả dụng, sử dụng SimpleVectorStore")
    from app.simple_vector_store import SimpleVectorStore
    vector_store_class = SimpleVectorStore

from app.config import settings
import json


def main():
    print("=" * 60)
    print("🚀 TRAINING SCRIPT - Mộc Châu Fruits AI")
    print("=" * 60)
    
    # Initialize vector store
    print("\n📦 Khởi tạo vector store...")
    vector_store = vector_store_class()
    
    # Check if data file exists
    if not os.path.exists(settings.DATA_PATH):
        print(f"❌ Không tìm thấy file dữ liệu: {settings.DATA_PATH}")
        print("Vui lòng tạo file dữ liệu trước khi training.")
        return
    
    # Load and display data info
    with open(settings.DATA_PATH, 'r', encoding='utf-8') as f:
        data = json.load(f)
    
    print(f"\n📊 Tìm thấy {len(data)} loại hoa quả trong file dữ liệu:")
    for fruit in data:
        print(f"   • {fruit['fruit_name']}")
    
    # Auto-confirm in production (no interactive input)
    import sys
    if sys.stdin.isatty():
        # Interactive mode (local)
        print("\n⚠️  Lưu ý: Quá trình này sẽ xóa dữ liệu cũ và load lại từ đầu.")
        confirm = input("Bạn có muốn tiếp tục? (y/n): ")
        
        if confirm.lower() != 'y':
            print("❌ Đã hủy.")
            return
    else:
        # Non-interactive mode (production/deploy)
        print("\n⚠️  Chế độ tự động: Đang load dữ liệu...")
    
    # Load data
    print("\n🔄 Đang load dữ liệu vào vector store...")
    try:
        count = vector_store.load_data_from_json()
        print(f"✅ Đã load thành công {count} documents!")
        
        # Verify
        total = vector_store.get_collection_count()
        print(f"📈 Tổng số documents trong vector store: {total}")
        
        # Test search
        print("\n🧪 Test tìm kiếm...")
        test_query = "vitamin C"
        results = vector_store.search(test_query, top_k=2)
        
        print(f"Tìm kiếm '{test_query}' - Tìm thấy {len(results)} kết quả:")
        for i, result in enumerate(results, 1):
            print(f"\n   Kết quả {i}:")
            print(f"   Hoa quả: {result['metadata']['fruit_name']}")
            print(f"   Độ liên quan: {1 - result['distance']:.2%}")
        
        print("\n" + "=" * 60)
        print("✨ HOÀN THÀNH! Hệ thống đã sẵn sàng.")
        print("=" * 60)
        print("\n💡 Bước tiếp theo:")
        print("   1. Chạy server: python -m uvicorn app.main:app --reload")
        print("   2. Truy cập: http://localhost:8000")
        print("   3. Bắt đầu chat với AI!")
        
    except Exception as e:
        print(f"❌ Lỗi: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()
