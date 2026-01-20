# Tổng quan kiến trúc & luồng code Mộc Vị Store AI

_File này giúp bạn hiểu nhanh hệ thống để dễ tự mở rộng._

---

## 1. Bức tranh tổng thể

Hệ thống AI được chia làm 3 phần chính:

- **[Dữ liệu & Vector Store]**: lưu trữ kiến thức về hoa quả Mộc Châu dưới dạng vector.
- **[LLM Service]**: gọi mô hình GPT/Gemini để sinh câu trả lời.
- **[API / UI / Script]**: cung cấp REST API (FastAPI) và script dòng lệnh để chat.

Luồng tổng quát khi người dùng hỏi:

1. Người dùng gửi câu hỏi qua:
   - REST API: `POST /api/chat` (file `app/main.py`), hoặc
   - Script dòng lệnh: `Trainning_AI/chat.py`.
2. Hệ thống dùng **Vector Store** để tìm các đoạn kiến thức liên quan.
3. Hệ thống gọi **LLMService** để sinh câu trả lời dựa trên câu hỏi + kết quả tìm kiếm.
4. Trả câu trả lời về cho người dùng (kèm thông tin nguồn).

---

## 2. Cấu hình chung – `app/config.py`

- Class `Settings` đọc config từ biến môi trường (`.env`) và đặt các giá trị mặc định.
- Một số thuộc tính chính:
  - `OPENAI_API_KEY`, `GEMINI_API_KEY`: khóa API cho mô hình AI.
  - `CHROMA_DB_PATH`, `COLLECTION_NAME`: nơi lưu ChromaDB.
  - `EMBEDDING_MODEL`: model tạo embedding (mặc định `sentence-transformers/all-MiniLM-L6-v2`).
  - `LLM_MODEL`: model LLM (ví dụ: `gpt-3.5-turbo`).
  - `DATA_PATH` + các path JSON khác: file dữ liệu hoa quả.

Bạn import cấu hình qua:

```python
from app.config import settings
```

---

## 3. Vector Store – `app/vector_store.py` & `app/simple_vector_store.py`

### 3.1. `VectorStore` (dùng ChromaDB)

- Khởi tạo client ChromaDB và collection:
  - `self.client = chromadb.PersistentClient(path=settings.CHROMA_DB_PATH, ...)`
  - `self.collection = self.client.get_or_create_collection(name=settings.COLLECTION_NAME, ...)`
- Hàm chính:
  - `_create_embeddings(texts)`: dùng `SentenceTransformer` tạo embedding.
  - `_format_fruit_data(fruit)`: convert 1 object hoa quả thành text dài, dễ hiểu cho model.
  - `load_data_from_json(json_path=None)`: đọc file JSON (mặc định `settings.DATA_PATH`), xóa collection cũ, tính embedding và add vào collection.
  - `search(query, top_k=None)`: tìm các documents liên quan, trả về list dict `{content, metadata, distance}`.
  - `add_custom_data(data)`: thêm một hoa quả mới vào collection.
  - `get_collection_count()`: trả số lượng documents.

### 3.2. `SimpleVectorStore` (dùng file `.pkl` – không cần ChromaDB)

- Dùng khi ChromaDB không tương thích (ví dụ Python 3.14).
- Lưu embeddings vào file `simple_vector_db.pkl`.
- Các bước tương tự `VectorStore` nhưng:
  - Dùng `numpy` để lưu mảng embeddings.
  - Tự tính khoảng cách cosine khi search.

### 3.3. Chọn class nào?

Ở các file khác (ví dụ `app/main.py`, `train.py`, `chat.py`), luôn có đoạn:

```python
try:
    from app.vector_store import VectorStore
    vector_store_class = VectorStore
except ImportError:
    from app.simple_vector_store import SimpleVectorStore
    vector_store_class = SimpleVectorStore
```

=> Nếu import `VectorStore` lỗi (không cài được ChromaDB) thì sẽ dùng `SimpleVectorStore`.

---

## 4. Dịch vụ LLM – `app/llm_service.py`

Class trung tâm: `LLMService`

### 4.1. Khởi tạo (`__init__`)

- Kiểm tra biến môi trường:
  - `OPENAI_API_KEY`
  - `GEMINI_API_KEY`
- Nếu có OpenAI:
  - Import `OpenAI`, tạo client `self.client = OpenAI(api_key=openai_key)`.
  - `self.model_type = "openai"`.
- Nếu không có OpenAI nhưng có Gemini:
  - Import `google.generativeai`, cấu hình API key.
  - Tạo `self.gemini_model = genai.GenerativeModel('gemini-2.0-flash')`.
  - `self.model_type = "gemini"`.
- Nếu không có API key hợp lệ: `self.model_type = "none"` và dùng chế độ trả lời đơn giản.

### 4.2. Hàm `chat(message, context=None, user_id="anonymous")`

- In log debug model đang dùng.
- Tùy `self.model_type` mà gọi:
  - `_chat_openai(...)`
  - `_chat_gemini(...)`
  - Hoặc `_simple_response(...)` nếu không có API.

### 4.3. Hàm `_chat_openai(...)`

- Tạo `system_prompt` mô tả rõ vai trò AI:
  - Chỉ trả lời về Mộc Vị Store, sản phẩm, tư vấn mua hàng…
  - Không trả lời các chủ đề ngoài phạm vi.
  - Có hướng dẫn cách thêm vào giỏ hàng khi khách muốn mua.
- Tạo `messages` gồm:
  - `{"role": "system", "content": system_prompt}`
  - `{"role": "user", "content": message}`
- Gọi API:

```python
response = self.client.chat.completions.create(
    model="gpt-4o-mini",
    messages=messages,
    max_tokens=500,
    temperature=0.7,
)
```

- Lấy `response.choices[0].message.content` làm câu trả lời.

### 4.4. Hàm `_chat_gemini(...)`

- Gọi `self.gemini_model.generate_content(message)` và trả `response.text`.

> Lưu ý: Trong `app/main.py` đang sử dụng các hàm `detect_purchase_intent` và `generate_response` của `LLMService`. Bạn có thể mở rộng/kiểm tra tiếp trong file `llm_service.py` nếu có phần dưới (scroll xuống nữa trong file).

---

## 5. FastAPI API – `app/main.py`

Đây là **điểm vào chính** khi chạy server bằng uvicorn.

### 5.1. Khởi tạo app

- Tạo `FastAPI` với title, description, docs.
- Thêm CORS cho phép mọi origin.
- Khởi tạo global:

```python
vector_store = vector_store_class()
llm_service = LLMService()
```

- Mount static UI nếu có thư mục `Trainning_AI/static`.

### 5.2. Model dữ liệu (Pydantic)

- `Message`: 1 tin nhắn trong history (`role`, `content`).
- `QueryRequest`: body của `POST /api/chat`.
- `QueryResponse`: cấu trúc trả về cho `/api/chat`.
- `OrderAction`: mô tả action mua hàng.
- `FruitData`: dữ liệu 1 loại hoa quả (dùng cho API thêm dữ liệu).
- `StatusResponse`: dùng cho việc reload dữ liệu.

### 5.3. Sự kiện startup

```python
@app.on_event("startup")
async def startup_event():
    count = vector_store.load_data_from_json()
```

- Khi server start, tự load dữ liệu vào vector store.

### 5.4. Các endpoint chính

- `GET /`:
  - Nếu có `static/index.html` thì trả file này (UI chat).
  - Nếu không thì trả JSON mô tả API.

- `GET /api/health`:
  - Trả trạng thái server và `documents_count` từ vector store.

- `POST /api/chat`:
  1. In debug câu hỏi và history.
  2. `search_results = vector_store.search(request.question, request.top_k)`.
  3. Nếu không có kết quả: trả lời mặc định.
  4. Gọi `llm_service.detect_purchase_intent(...)` để xem người dùng có ý định mua hàng.
  5. Gọi `llm_service.generate_response(question, search_results, conversation_history, purchase_intent)` để sinh câu trả lời.
  6. Format `sources` từ `search_results` (tên hoa quả, điểm liên quan).
  7. Nếu có `purchase_intent`, chuẩn bị `action` dạng `{'type': 'add_to_cart', 'products': [...]}`.
  8. Trả `QueryResponse`.

- `POST /api/train/reload`:
  - Gọi `vector_store.load_data_from_json()` để reload dữ liệu từ file JSON.

- `POST /api/train/add`:
  - Nhận 1 `FruitData` mới.
  - Gọi `vector_store.add_custom_data(...)` để thêm vào vector store.
  - Ghi thêm dữ liệu vào file JSON tại `settings.DATA_PATH`.

- `GET /api/fruits`:
  - Đọc file JSON và trả về list tất cả hoa quả.

### 5.5. Chạy trực tiếp file `main.py`

Ở cuối file:

```python
if __name__ == "__main__":
    import uvicorn
    ...
    uvicorn.run(app, host=host, port=port, reload=False)
```

=> Bạn có thể chạy server bằng:

```bash
python app/main.py
# hoặc
uvicorn app.main:app --reload
```

---

## 6. Script training – `Trainning_AI/train.py`

Mục đích: **load dữ liệu từ JSON vào vector store** (ChromaDB hoặc SimpleVectorStore).

Luồng chính:

1. Thêm `Trainning_AI` vào `sys.path` để import được `app.*`.
2. Chọn `vector_store_class` như đã giải thích ở trên.
3. Kiểm tra file dữ liệu `settings.DATA_PATH` có tồn tại.
4. Đọc file JSON, in ra danh sách hoa quả.
5. Hỏi confirm (nếu chạy ở chế độ tương tác) vì sẽ xóa dữ liệu cũ.
6. Gọi `vector_store.load_data_from_json()`.
7. In số lượng documents và test search nhanh.
8. Gợi ý các bước tiếp theo: chạy server và chat.

Bạn thường chạy:

```bash
python Trainning_AI/train.py
```

trước khi chạy server lần đầu.

---

## 7. Script chat dòng lệnh – `Trainning_AI/chat.py`

- Cho phép bạn chat trực tiếp trong terminal, không cần front-end.

Luồng chính:

1. Thêm thư mục `Trainning_AI` vào `sys.path`.
2. Chọn `vector_store_class` (Chroma hay Simple).
3. Khởi tạo `vector_store` và `LLMService`.
4. Kiểm tra `vector_store.get_collection_count()`:
   - Nếu `0` thì yêu cầu chạy `python train.py` trước.
5. Vòng lặp:
   - Nhập câu hỏi từ `input("💬 Bạn: ")`.
   - Nếu người dùng gõ `exit/quit/thoát/bye` thì thoát.
   - Gọi `vector_store.search(question, top_k=...)`.
   - Gọi `llm_service.generate_response(question, results)`.
   - In câu trả lời + hiển thị top nguồn tham khảo.

---

## 8. Gợi ý cách tự mở rộng code

Dưới đây là một số ví dụ cụ thể:

### 8.1. Thêm endpoint API mới

Ví dụ bạn muốn thêm endpoint `GET /api/prices` trả về danh sách giá cơ bản:

1. Mở `app/main.py`.
2. Sau `@app.get("/api/fruits")`, bạn có thể thêm:

```python
@app.get("/api/prices")
async def get_prices():
    # Đọc dữ liệu từ settings.DATA_PATH giống như /api/fruits
    # Sau đó chỉ lọc ra thông tin giá và trả về
    ...
```

### 8.2. Thêm logic mới vào `LLMService`

- Bạn có thể chỉnh `system_prompt` trong `_chat_openai` để thay đổi cách AI nói chuyện.
- Bạn có thể:
  - Thêm function mới, ví dụ `summarize_product_list(results)`.
  - Gọi function đó trong `generate_response` để format lại context trước khi gửi cho LLM.

### 8.3. Thêm trường mới vào dữ liệu hoa quả

1. Thêm field mới trong JSON (ví dụ `"origin": "Mộc Châu"`).
2. Cập nhật:
   - `FruitData` trong `app/main.py`.
   - `_format_fruit_data` trong `vector_store.py` và `simple_vector_store.py` để đưa field mới vào text embedding.
3. Nếu cần, chỉnh front-end (nếu bạn có UI riêng) để hiển thị thông tin mới.

---

## 9. Tóm tắt ngắn gọn

- **Chạy training**: `python Trainning_AI/train.py`.
- **Chạy API**: `uvicorn app.main:app --reload` (từ thư mục `Trainning_AI`).
- **Script chat**: `python Trainning_AI/chat.py`.
- Code chia thành: `config` (cấu hình), `vector_store` (kiến thức), `llm_service` (gọi AI), `main.py` (API), `train.py` & `chat.py` (script tiện ích).

Bạn có thể chỉnh/sao chép các pattern có sẵn để thêm API mới, thêm logic AI hoặc mở rộng dữ liệu.

---

## 10. Triển khai web AI lên Internet

### 10.1. Kiến trúc web trong dự án

- **Backend FastAPI** (`Trainning_AI/app/main.py`):
  - Chạy server API và (nếu có) trả giao diện web chat.
  - Endpoint chính:
    - `GET /` → nếu có `static/index.html` thì trả UI chat, nếu không thì trả JSON mô tả API.
    - `POST /api/chat` → nhận câu hỏi và trả lời từ AI.
    - `GET /api/fruits`, `POST /api/train/reload`, `POST /api/train/add` → quản lý dữ liệu hoa quả.
  - Khởi tạo:
    - `vector_store = vector_store_class()` → load dữ liệu hoa quả vào vector store.
    - `llm_service = LLMService()` → chuẩn bị kết nối OpenAI/Gemini.

- **LLM + Vector Store phía sau**:
  - `llm_service.py`: tự động chọn OpenAI hoặc Gemini tùy API key.
  - `vector_store.py` / `simple_vector_store.py`: lưu kiến thức hoa quả dạng vector để tìm kiếm nhanh.

- **Web UI (nếu có)**:
  - Thư mục `Trainning_AI/static` (nếu bạn tạo): chứa `index.html`, CSS, JS.
  - Frontend gọi `POST /api/chat` để gửi câu hỏi và hiển thị câu trả lời.

Khi deploy, bạn sẽ đưa **cả server FastAPI** lên Internet. Người dùng truy cập URL (hoặc domain) → gửi request đến `/api/chat` hoặc vào giao diện web (nếu có static UI).

### 10.2. Những thứ cần chuẩn bị/mua

#### 1) Dịch vụ chạy ứng dụng (hosting)

- **PaaS (đề xuất cho bạn)** – ví dụ: Render.com, Railway.app, Fly.io...
  - Ưu điểm: dễ dùng, tự có HTTPS, có domain miễn phí (`https://ten-service.onrender.com`).
  - Nhược điểm: cần tài khoản + thường yêu cầu thẻ, free tier có giới hạn.

- **VPS (DigitalOcean, Vultr, Linode, Hetzner...)** 
  - Mua 1 server ảo (thường 5–10 USD/tháng).
  - Bạn phải tự cài Python/Docker, cấu hình uvicorn + Nginx + SSL.
  - Mạnh và linh hoạt nhưng phức tạp hơn.

**Gợi ý**: nếu bạn chỉ cần chạy chatbot Mộc Vị Store cho khách dùng → chọn **PaaS (Render hoặc Railway)** để tiết kiệm thởi gian.

#### 2) Domain

- Không bắt buộc lúc đầu. Bạn có thể dùng domain miễn phí do platform cấp.
- Khi muốn chuyên nghiêp hơn:
  - Mua domain ở Namecheap, Porkbun, Cloudflare Registrar...
  - Giá khoảng 10–15 USD/năm cho `.com`.
  - Trỏ DNS của domain về service trên Render/Railway.

#### 3) API key cho AI

- **OpenAI (ChatGPT)**:
  - Tạo tài khoản ở https://platform.openai.com.
  - Tạo API key, gán vào biến môi trường `OPENAI_API_KEY`.
  - Tính tiền theo số token dùng, nên cần add thẻ và quản lý usage.

- **Google Gemini**:
  - Tạo key ở https://aistudio.google.com.
  - Gán vào biến môi trường `GEMINI_API_KEY`.
  - Có free tier (một số request miễn phí mỗi ngày).

Trong `LLMService`:

- Nếu có `OPENAI_API_KEY` → ưu tiên dùng OpenAI.
- Nếu không có, nhưng có `GEMINI_API_KEY` → dùng Gemini.
- Nếu không có key nào → dùng chế độ trả lời đơn giản.

### 10.3. Quy trình deploy đề xuất (Render.com)

#### Bước 1: Chuẩn bị code

- Đảm bảo có file `requirements.txt` chứa các thư viện cần thiết, ví dụ:
  - `fastapi`, `uvicorn[standard]`, `python-dotenv`,
  - `chromadb` (nếu dùng), `sentence-transformers`,
  - `openai`, `google-generativeai`, v.v.
- Đưa toàn bộ project lên GitHub (tạo repo mới, commit & push).

#### Bước 2: Tạo Web Service trên Render

1. Đăng nhập Render → chọn **New** → **Web Service**.
2. Kết nối với GitHub, chọn repo chứa project.
3. Thiết lập:
   - **Environment**: Python.
   - **Build Command**: `pip install -r requirements.txt`.
   - **Root Directory**: nếu code FastAPI nằm ở `Trainning_AI`, có thể set root = `Trainning_AI`.
   - **Start Command**, ví dụ:

```bash
uvicorn app.main:app --host 0.0.0.0 --port 10000
```

4. Thiết lập **Environment Variables** trên Render:
   - `OPENAI_API_KEY` = key OpenAI của bạn (nếu dùng).
   - `GEMINI_API_KEY` = key Gemini (nếu dùng).
   - Có thể thêm `HOST=0.0.0.0`, `PORT` nếu cần (thường Render cung cấp sẵn `PORT`).
5. Deploy, chờ build & run thành công.
6. Render sẽ cho bạn 1 URL, ví dụ: `https://mocvi-ai.onrender.com`.

#### Bước 3: Kiểm tra sau khi deploy

- Mở:
  - `https://<tên-service>.onrender.com/docs` → Swagger UI để test API.
  - `GET /api/health` → kiểm tra trạng thái.
  - `POST /api/chat` → test chat với AI.
- Nếu có `static/index.html` → truy cập root `/` sẽ mở UI chat.

### 10.4. Thêm domain riêng (tùy chọn)

1. Mua domain (ví dụ `mocvistore.com`).
2. Trong Render, vào service → **Custom Domains** → thêm `chat.mocvistore.com`.
3. Render sẽ hiển thị hướng dẫn DNS (CNAME/A record).
4. Vào trang quản lý domain (Namecheap/Porkbun/Cloudflare...) → thêm DNS record như Render yêu cầu.
5. Đợi DNS cập nhật (vài phút đến vài giờ). Sau đó truy cập:
   - `https://chat.mocvistore.com` → trang web AI của bạn.

### 10.5. Tóm tắt triển khai

- Chuẩn bị: API key (OpenAI/Gemini), GitHub, `requirements.txt`.
- Dùng PaaS (Render/Railway) để chạy FastAPI:
  - Build: `pip install -r requirements.txt`.
  - Start: `uvicorn app.main:app --host 0.0.0.0 --port <PORT>`.
- Test `/docs` và `/api/chat` sau khi deploy.
- Khi ổn: mua domain, trỏ DNS về service để có URL đẹp cho khách hàng.
