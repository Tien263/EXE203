import json
import chromadb
from chromadb.config import Settings as ChromaSettings
from sentence_transformers import SentenceTransformer
from typing import List, Dict
from app.config import settings


class VectorStore:
    def __init__(self):
        """Khởi tạo vector store với ChromaDB"""
        self.client = chromadb.PersistentClient(
            path=settings.CHROMA_DB_PATH,
            settings=ChromaSettings(anonymized_telemetry=False)
        )
        
        # Load embedding model
        self.embedding_model = SentenceTransformer(settings.EMBEDDING_MODEL)
        
        # Get or create collection
        self.collection = self.client.get_or_create_collection(
            name=settings.COLLECTION_NAME,
            metadata={"description": "Mộc Châu fruits knowledge base"}
        )
    
    def _create_embeddings(self, texts: List[str]) -> List[List[float]]:
        """Tạo embeddings từ texts"""
        embeddings = self.embedding_model.encode(texts)
        return embeddings.tolist()
    
    def _format_fruit_data(self, fruit: Dict) -> str:
        """Format dữ liệu hoa quả thành text để embedding"""
        text_parts = [
            f"Tên: {fruit['fruit_name']}",
            f"Mô tả: {fruit['description']}",
            f"Mùa vụ: {fruit['season']}",
            f"Cách sử dụng: {fruit['usage']}",
            "\nThành phần dinh dưỡng:"
        ]
        
        # Add nutrients
        for nutrient, benefit in fruit['nutrients'].items():
            text_parts.append(f"- {nutrient.replace('_', ' ').title()}: {benefit}")
        
        # Add health benefits
        text_parts.append("\nLợi ích sức khỏe:")
        for benefit in fruit['health_benefits']:
            text_parts.append(f"- {benefit}")
        
        return "\n".join(text_parts)
    
    def load_data_from_json(self, json_path: str = None):
        """Load dữ liệu từ file JSON và lưu vào vector store"""
        if json_path is None:
            json_path = settings.DATA_PATH
        
        with open(json_path, 'r', encoding='utf-8') as f:
            fruits_data = json.load(f)
        
        # Clear existing data
        try:
            self.client.delete_collection(settings.COLLECTION_NAME)
            self.collection = self.client.create_collection(
                name=settings.COLLECTION_NAME,
                metadata={"description": "Mộc Châu fruits knowledge base"}
            )
        except:
            pass
        
        # Prepare data for insertion
        documents = []
        metadatas = []
        ids = []
        
        for fruit in fruits_data:
            doc_text = self._format_fruit_data(fruit)
            documents.append(doc_text)
            metadatas.append({
                "fruit_name": fruit['fruit_name'],
                "season": fruit['season'],
                "id": fruit['id']
            })
            ids.append(fruit['id'])
        
        # Create embeddings
        embeddings = self._create_embeddings(documents)
        
        # Add to collection
        self.collection.add(
            documents=documents,
            embeddings=embeddings,
            metadatas=metadatas,
            ids=ids
        )
        
        return len(documents)
    
    def search(self, query: str, top_k: int = None) -> List[Dict]:
        """Tìm kiếm thông tin liên quan đến query"""
        if top_k is None:
            top_k = settings.TOP_K_RESULTS
        
        # Create query embedding
        query_embedding = self._create_embeddings([query])[0]
        
        # Search in collection
        results = self.collection.query(
            query_embeddings=[query_embedding],
            n_results=top_k
        )
        
        # Format results
        formatted_results = []
        if results['documents'] and len(results['documents'][0]) > 0:
            for i in range(len(results['documents'][0])):
                formatted_results.append({
                    "content": results['documents'][0][i],
                    "metadata": results['metadatas'][0][i],
                    "distance": results['distances'][0][i] if 'distances' in results else None
                })
        
        return formatted_results
    
    def add_custom_data(self, data: Dict):
        """Thêm dữ liệu tùy chỉnh vào vector store"""
        doc_text = self._format_fruit_data(data)
        embedding = self._create_embeddings([doc_text])[0]
        
        self.collection.add(
            documents=[doc_text],
            embeddings=[embedding],
            metadatas=[{
                "fruit_name": data['fruit_name'],
                "season": data.get('season', 'N/A'),
                "id": data['id']
            }],
            ids=[data['id']]
        )
        
        return True
    
    def get_collection_count(self) -> int:
        """Lấy số lượng documents trong collection"""
        return self.collection.count()
