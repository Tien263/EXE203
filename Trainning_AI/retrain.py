# -*- coding: utf-8 -*-
"""Retrain vector database with all products"""
import sys
import os
sys.path.insert(0, os.path.dirname(__file__))

from app.simple_vector_store import SimpleVectorStore

print("Retraining vector database...\n")

# Initialize
vector_store = SimpleVectorStore()

# Load all data files
data_files = [
    "data/moc_chau_fruits.json",
    "data/brand_info.json",
    "data/seasonal_calendar.json",
    "data/storage_guide.json"
]

total_docs = 0
for file in data_files:
    if os.path.exists(file):
        print(f"Loading {file}...")
        count = vector_store.load_data_from_json(file)
        total_docs += count
        print(f"   Loaded {count} documents")
    else:
        print(f"   File not found: {file}")

print(f"\nTotal: {total_docs} documents loaded!")
print(f"Saved to: simple_vector_db.pkl")
print("\nTraining completed! You can now use chat.py")
