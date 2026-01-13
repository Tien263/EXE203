# -*- coding: utf-8 -*-
"""Test multilingual support - Vietnamese & English"""
import sys
import os
sys.path.insert(0, os.path.dirname(__file__))

from app.llm_service import LLMService
from app.simple_vector_store import SimpleVectorStore

print("🌐 Testing Multilingual AI\n")

# Initialize
vector_store = SimpleVectorStore()
llm_service = LLMService()

# Load data
print("📚 Loading data...")
vector_store.load_from_json("data/moc_chau_fruits.json")
print(f"✅ Loaded {len(vector_store.documents)} documents\n")

# Test cases
tests = [
    ("🇻🇳 Vietnamese", "Dâu tây có tốt cho sức khỏe không?"),
    ("🇬🇧 English", "What are the health benefits of strawberries?"),
    ("🇻🇳 Vietnamese", "100.000đ mua được gì?"),
    ("🇬🇧 English", "What can I buy with 100,000 VND?"),
]

for lang, question in tests:
    print(f"\n{'='*60}")
    print(f"{lang}")
    print(f"❓ {question}")
    print(f"{'='*60}\n")
    
    results = vector_store.search(question, top_k=3)
    
    if results:
        print("💬 AI: ", end='', flush=True)
        answer = llm_service.generate_response(question, results)
        print()
    
    input("\n▶ Press Enter to continue...")

print("\n✅ Test completed!")
