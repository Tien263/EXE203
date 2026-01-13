"""
Script test API - Ví dụ cách gọi API
"""
import requests
import json

# Base URL
BASE_URL = "http://localhost:8000"

def test_health():
    """Test health check endpoint"""
    print("\n" + "="*60)
    print("TEST 1: Health Check")
    print("="*60)
    
    response = requests.get(f"{BASE_URL}/api/health")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2, ensure_ascii=False)}")
    return response.status_code == 200

def test_chat(question):
    """Test chat endpoint"""
    print("\n" + "="*60)
    print(f"TEST 2: Chat - '{question}'")
    print("="*60)
    
    payload = {
        "question": question,
        "top_k": 3
    }
    
    response = requests.post(
        f"{BASE_URL}/api/chat",
        json=payload,
        headers={"Content-Type": "application/json"}
    )
    
    print(f"Status Code: {response.status_code}")
    
    if response.status_code == 200:
        data = response.json()
        print(f"\nCâu trả lời:")
        print(data['answer'])
        print(f"\nNguồn tham khảo:")
        for source in data['sources']:
            print(f"  - {source['fruit_name']} (độ liên quan: {source['relevance_score']:.2%})")
    else:
        print(f"Error: {response.text}")
    
    return response.status_code == 200

def test_get_fruits():
    """Test get all fruits endpoint"""
    print("\n" + "="*60)
    print("TEST 3: Get All Fruits")
    print("="*60)
    
    response = requests.get(f"{BASE_URL}/api/fruits")
    print(f"Status Code: {response.status_code}")
    
    if response.status_code == 200:
        fruits = response.json()
        print(f"\nTìm thấy {len(fruits)} loại hoa quả:")
        for fruit in fruits:
            print(f"  - {fruit['fruit_name']} (Mùa: {fruit['season']})")
    else:
        print(f"Error: {response.text}")
    
    return response.status_code == 200

def test_add_fruit():
    """Test add fruit endpoint"""
    print("\n" + "="*60)
    print("TEST 4: Add New Fruit")
    print("="*60)
    
    new_fruit = {
        "id": "test_001",
        "fruit_name": "Cam Mộc Châu (Test)",
        "description": "Cam Mộc Châu có vị ngọt thanh, giàu vitamin C",
        "nutrients": {
            "vitamin_C": "Rất cao, tăng cường miễn dịch",
            "kali": "Tốt cho tim mạch"
        },
        "health_benefits": [
            "Tăng cường hệ miễn dịch",
            "Chống oxy hóa"
        ],
        "season": "Tháng 10-12",
        "usage": "Ăn tươi, vắt nước"
    }
    
    response = requests.post(
        f"{BASE_URL}/api/train/add",
        json=new_fruit,
        headers={"Content-Type": "application/json"}
    )
    
    print(f"Status Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2, ensure_ascii=False)}")
    
    return response.status_code == 200

def test_reload_data():
    """Test reload data endpoint"""
    print("\n" + "="*60)
    print("TEST 5: Reload Data")
    print("="*60)
    
    response = requests.post(f"{BASE_URL}/api/train/reload")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2, ensure_ascii=False)}")
    
    return response.status_code == 200

def main():
    """Run all tests"""
    print("\n" + "🧪 BẮT ĐẦU TEST API ".center(60, "="))
    print("Đảm bảo server đang chạy tại http://localhost:8000")
    
    try:
        # Test 1: Health check
        test_health()
        
        # Test 2: Chat
        test_chat("Mận Mộc Châu có những thành phần dinh dưỡng gì?")
        
        # Test 3: Get all fruits
        test_get_fruits()
        
        # Test 4: Add fruit (commented out to avoid adding test data)
        # test_add_fruit()
        
        # Test 5: Reload data (commented out)
        # test_reload_data()
        
        print("\n" + "✅ HOÀN THÀNH TẤT CẢ TESTS ".center(60, "="))
        
    except requests.exceptions.ConnectionError:
        print("\n❌ LỖI: Không thể kết nối đến server!")
        print("Vui lòng chạy server trước: python -m uvicorn app.main:app --reload")
    except Exception as e:
        print(f"\n❌ LỖI: {e}")

if __name__ == "__main__":
    main()
