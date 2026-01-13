"""
Simple vector store implementation without ChromaDB
Sử dụng cho Python 3.14 khi ChromaDB chưa tương thích
"""
import json
import pickle
import os
from typing import List, Dict
from sentence_transformers import SentenceTransformer
import numpy as np
from app.config import settings


class SimpleVectorStore:
    def __init__(self):
        """Khởi tạo simple vector store"""
        self.embedding_model = SentenceTransformer(settings.EMBEDDING_MODEL)
        self.documents = []
        self.embeddings = []
        self.metadatas = []
        self.ids = []
        
        # Load từ file nếu có
        self.db_file = "simple_vector_db.pkl"
        self._load_from_file()
    
    def _format_fruit_data(self, fruit: Dict) -> str:
        """Format dữ liệu hoa quả thành text để embedding"""
        fruit_name = fruit['fruit_name']
        # Lặp lại tên quả nhiều lần để embedding học tốt hơn
        text_parts = [
            f"Tên sản phẩm: {fruit_name}",
            f"{fruit_name}",
            f"Mô tả: {fruit['description']}",
            f"{fruit_name}",
        ]
        
        # Add category if exists
        if 'category' in fruit:
            text_parts.append(f"Loại: {fruit['category']}")
        
        # Add selling points if exists
        if 'selling_points' in fruit:
            text_parts.append(f"\nĐiểm nổi bật của {fruit_name}:")
            for point in fruit['selling_points']:
                text_parts.append(f"- {point}")
        
        # Add nutrients
        if 'nutrients' in fruit:
            text_parts.append(f"\nThành phần dinh dưỡng của {fruit_name}:")
            for nutrient, benefit in fruit['nutrients'].items():
                text_parts.append(f"- {nutrient.replace('_', ' ').title()}: {benefit}")
        
        # Add health benefits
        if 'health_benefits' in fruit:
            text_parts.append(f"\nLợi ích sức khỏe của {fruit_name}:")
            for benefit in fruit['health_benefits']:
                text_parts.append(f"- {benefit}")
        
        # Add season
        if 'season' in fruit:
            text_parts.append(f"\nMùa vụ: {fruit['season']}")
        
        # Add usage ideas if exists
        if 'usage_ideas' in fruit:
            text_parts.append(f"\nCách sử dụng {fruit_name}:")
            for idea in fruit['usage_ideas']:
                text_parts.append(f"- {idea}")
        elif 'usage' in fruit:
            text_parts.append(f"\nCách sử dụng: {fruit['usage']}")
        
        # Add price if exists
        if 'price' in fruit:
            text_parts.append(f"\nGiá bán {fruit_name}:")
            for size, price in fruit['price'].items():
                text_parts.append(f"- {size}: {price}")
        
        # Add storage if exists
        if 'storage' in fruit:
            text_parts.append(f"\nBảo quản: {fruit['storage']}")
        
        # Add target customers if exists
        if 'target_customers' in fruit:
            text_parts.append(f"\nKhách hàng mục tiêu: {fruit['target_customers']}")
        
        # Add promotion if exists
        if 'promotion' in fruit:
            text_parts.append(f"\nKhuyến mãi: {fruit['promotion']}")
        
        return "\n".join(text_parts)
    
    def load_data_from_json(self, json_path: str = None):
        """Load dữ liệu từ file JSON"""
        if json_path is None:
            json_path = settings.DATA_PATH
        
        print(f"[DEBUG] Trying to load data from: {json_path}")
        print(f"[DEBUG] File exists: {os.path.exists(json_path)}")
        
        if not os.path.exists(json_path):
            print(f"[ERROR] File not found: {json_path}")
            return 0
        
        with open(json_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        print(f"[DEBUG] Loaded data type: {type(data)}")
        
        # Check if data is array or single object
        if isinstance(data, list):
            fruits_data = data
        else:
            # Single object (like brand_info) - skip for now
            return 0
        
        # Clear existing data before loading new data
        self.documents = []
        self.embeddings = []
        self.metadatas = []
        self.ids = []
        
        # Process each fruit
        for fruit in fruits_data:
            doc_text = self._format_fruit_data(fruit)
            embedding = self.embedding_model.encode(doc_text)
            
            self.documents.append(doc_text)
            self.embeddings.append(embedding)
            self.metadatas.append({
                "fruit_name": fruit['fruit_name'],
                "season": fruit.get('season', 'Quanh năm'),
                "id": fruit['id']
            })
            self.ids.append(fruit['id'])
        
        # Convert to numpy array for faster computation
        self.embeddings = np.array(self.embeddings)
        
        # Save to file
        self._save_to_file()
        
        # Return total number of documents
        return len(self.documents)
    
    def search(self, query: str, top_k: int = None) -> List[Dict]:
        """Tìm kiếm thông tin liên quan đến query"""
        if top_k is None:
            top_k = settings.TOP_K_RESULTS
        
        if len(self.documents) == 0:
            return []
        
        # Create query embedding
        query_embedding = self.embedding_model.encode(query)
        
        # Calculate cosine similarity
        similarities = np.dot(self.embeddings, query_embedding) / (
            np.linalg.norm(self.embeddings, axis=1) * np.linalg.norm(query_embedding)
        )
        
        # Get top k indices
        top_indices = np.argsort(similarities)[::-1][:top_k]
        
        # Format results
        results = []
        for idx in top_indices:
            similarity = float(similarities[idx])
            results.append({
                "content": self.documents[idx],
                "metadata": self.metadatas[idx],
                "distance": float(1 - similarity)  # Convert similarity to distance
            })
        
        return results
    
    def add_custom_data(self, data: Dict):
        """Thêm dữ liệu tùy chỉnh"""
        doc_text = self._format_fruit_data(data)
        embedding = self.embedding_model.encode(doc_text)
        
        self.documents.append(doc_text)
        self.embeddings = np.vstack([self.embeddings, embedding]) if len(self.embeddings) > 0 else np.array([embedding])
        self.metadatas.append({
            "fruit_name": data['fruit_name'],
            "season": data.get('season', 'N/A'),
            "id": data['id']
        })
        self.ids.append(data['id'])
        
        self._save_to_file()
        return True
    
    def get_collection_count(self) -> int:
        """Lấy số lượng documents"""
        return len(self.documents)
    
    def _save_to_file(self):
        """Lưu vector store vào file"""
        data = {
            'documents': self.documents,
            'embeddings': self.embeddings,
            'metadatas': self.metadatas,
            'ids': self.ids
        }
        with open(self.db_file, 'wb') as f:
            pickle.dump(data, f)
    
    def _load_from_file(self):
        """Load vector store từ file"""
        if os.path.exists(self.db_file):
            try:
                with open(self.db_file, 'rb') as f:
                    data = pickle.load(f)
                self.documents = data['documents']
                self.embeddings = data['embeddings']
                self.metadatas = data['metadatas']
                self.ids = data['ids']
                print(f"Loaded {len(self.documents)} documents from cache")
            except Exception as e:
                print(f"Could not load cache: {e}")
