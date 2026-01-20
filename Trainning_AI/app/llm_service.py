from typing import List, Dict, Optional
import os
import json
import re


class LLMService:
    def __init__(self):
        """Khởi tạo LLM service với debug chi tiết"""
        self.client = None
        self.gemini_model = None
        self.model_type = "none"
        
        print(f"[DEBUG] Initializing LLM Service...")
        
        # Check environment variables
        openai_key = os.getenv("OPENAI_API_KEY", "")
        gemini_key = os.getenv("GEMINI_API_KEY", "")
        
        print(f"[DEBUG] OPENAI_API_KEY: {'Found' if openai_key else 'Not found'}")
        print(f"[DEBUG] GEMINI_API_KEY: {'Found' if gemini_key else 'Not found'}")
        
        # Try OpenAI first
        if openai_key:
            try:
                print("[DEBUG] Attempting to import OpenAI...")
                from openai import OpenAI
                print("[DEBUG] OpenAI imported successfully")
                
                print("[DEBUG] Creating OpenAI client...")
                self.client = OpenAI(api_key=openai_key)
                self.model_type = "openai"
                print("[OK] ✅ Sử dụng OpenAI ChatGPT - Chất lượng cao nhất!")
                return
                
            except ImportError as e:
                print(f"[ERROR] OpenAI import failed: {e}")
            except Exception as e:
                print(f"[ERROR] OpenAI initialization failed: {e}")
        else:
            print("[DEBUG] No OpenAI API key found")
        
        # Try Gemini as fallback
        if gemini_key:
            try:
                print("[DEBUG] Attempting to use Gemini...")
                import google.generativeai as genai
                genai.configure(api_key=gemini_key)
                self.gemini_model = genai.GenerativeModel('gemini-2.0-flash')
                self.model_type = "gemini"
                print("[OK] Sử dụng Google Gemini 2.0 Flash (miễn phí)")
                return
            except Exception as e:
                print(f"[ERROR] Gemini initialization failed: {e}")
        
        print("[WARNING] Không có AI API key hợp lệ. Sử dụng chế độ simple response.")

    def chat(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với AI - Có debug chi tiết"""
        print(f"[DEBUG] Chat called with model_type: {self.model_type}")
        print(f"[DEBUG] Message: {message}")
        
        try:
            if self.model_type == "openai":
                return self._chat_openai(message, context, user_id)
            elif self.model_type == "gemini":
                return self._chat_gemini(message, context, user_id)
            else:
                return self._simple_response(message)
        except Exception as e:
            print(f"[ERROR] Chat error: {e}")
            import traceback
            traceback.print_exc()
            return f"Xin lỗi, tôi gặp lỗi kỹ thuật: {str(e)}"

    def _chat_openai(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với OpenAI ChatGPT"""
        print("[DEBUG] Using OpenAI chat")
        
        try:
            # Tạo messages đơn giản
            system_prompt = """Bạn là AI assistant CHUYÊN BIỆT của Mộc Vị Store - cửa hàng hoa quả sấy cao cấp từ Mộc Châu.

🎯 PHẠM VI HỖ TRỢ:
✅ CÓ THỂ trả lời:
- Giới thiệu bản thân AI và Mộc Vị Store
- Thông tin về thương hiệu, cửa hàng, nguồn gốc sản phẩm
- Sản phẩm hoa quả sấy và dịch vụ
- Tư vấn mua hàng, so sánh sản phẩm
- Hướng dẫn sử dụng, bảo quản
- Chính sách đổi trả, giao hàng

❌ KHÔNG trả lời:
- Toán học, khoa học, lịch sử
- Thời tiết, tin tức, chính trị
- Lời khuyên y tế, pháp lý
- Chủ đề không liên quan đến cửa hàng

🏪 GIỚI THIỆU MỘC VỊ STORE:
- Thương hiệu hoa quả sấy cao cấp từ Mộc Châu
- Sản phẩm 100% tự nhiên, không chất bảo quản
- Công nghệ sấy hiện đại, giữ nguyên dinh dưỡng
- Phục vụ khách hàng yêu thích sản phẩm chất lượng

🛒 CHỨC NĂNG THÊM VÀO GIỎ HÀNG:
- Khi khách muốn mua/đặt hàng, hãy xác nhận sản phẩm và số lượng
- Sau đó nói: "Tôi đã thêm [sản phẩm] vào giỏ hàng cho bạn!"
- Đưa ra tổng tiền và hướng dẫn thanh toán

🍓 SẢN PHẨM CHÍNH:
1. **Sấy Dẻo (200g)**: Mận (65k), Xoài (70k), Đào (65k), Dâu (90k), Hồng (95k)
2. **Sấy Giòn (200g)**: Mít (80k), Chuối (80k)  
3. **Sấy Thăng Hoa (100g)**: Dâu (140k), Sữa chua (95k)
4. **Mini Mix (50g)**: Tất cả loại trên, tối thiểu 4 pack

Hãy thân thiện, nhiệt tình và tập trung vào việc hỗ trợ khách hàng tốt nhất!"""

            messages = [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": message}
            ]
            
            print(f"[DEBUG] Calling OpenAI API with {len(messages)} messages")
            
            # Gọi OpenAI API
            response = self.client.chat.completions.create(
                model="gpt-4o-mini",
                messages=messages,
                max_tokens=500,
                temperature=0.7
            )
            
            result = response.choices[0].message.content.strip()
            print(f"[DEBUG] OpenAI response: {result[:100]}...")
            return result
            
        except Exception as e:
            print(f"[ERROR] OpenAI chat error: {e}")
            import traceback
            traceback.print_exc()
            return f"Lỗi OpenAI: {str(e)}"

    def _chat_gemini(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với Gemini"""
        print("[DEBUG] Using Gemini chat")
        try:
            response = self.gemini_model.generate_content(message)
            return response.text.strip()
        except Exception as e:
            print(f"[ERROR] Gemini chat error: {e}")
            return f"Lỗi Gemini: {str(e)}"

    def _simple_response(self, message: str) -> str:
        """Phản hồi đơn giản khi không có AI"""
        print("[DEBUG] Using simple response")
        
        message_lower = message.lower()
        
        if any(word in message_lower for word in ['xin chào', 'hello', 'hi', 'chào']):
            return """Xin chào! 👋 Chào mừng bạn đến với Mộc Vị Store! 
            
Tôi có thể giúp bạn tìm hiểu về các sản phẩm hoa quả sấy cao cấp từ Mộc Châu:
🍓 Sấy dẻo: Mận, Xoài, Đào, Dâu, Hồng
🥭 Sấy giòn: Mít, Chuối  
✨ Sấy thăng hoa: Dâu, Sữa chua

Bạn quan tâm loại nào nhất? 😊"""
        else:
            return f"Cảm ơn bạn đã nhắn tin: '{message}'. Tôi là AI assistant của Mộc Vị Store, sẵn sàng hỗ trợ bạn về các sản phẩm hoa quả sấy! 😊"

    def detect_purchase_intent(self, query: str) -> Dict:
        """Phát hiện ý định mua hàng và trích xuất sản phẩm"""
        print(f"[DEBUG] Detecting purchase intent for: {query}")
        
        query_lower = query.lower()
        purchase_keywords = ['mua', 'đặt', 'order', 'thêm vào giỏ', 'cho vào giỏ', 'lấy', 'gói', 'đặt hàng']
        
        # Kiểm tra ý định mua hàng
        has_purchase_intent = any(keyword in query_lower for keyword in purchase_keywords)
        
        if not has_purchase_intent:
            return {'is_purchase': False, 'products': [], 'confidence': 0.1}
        
        # Trích xuất sản phẩm bằng OpenAI nếu có ý định mua hàng
        if self.model_type == "openai" and has_purchase_intent:
            try:
                extract_prompt = f"""Phân tích câu sau để trích xuất thông tin đặt hàng:
"{query}"

Trả về JSON format:
{{
    "is_purchase": true/false,
    "products": [
        {{"name": "tên sản phẩm chính xác", "quantity": số_lượng, "size": "200g hoặc 100g hoặc 50g"}}
    ]
}}

DANH SÁCH SẢN PHẨM CHÍNH XÁC:
- Mận Sấy Dẻo (200g: 65k, 50g: 18k)
- Xoài Sấy Dẻo (200g: 70k, 50g: 20k)
- Đào Sấy Dẻo (200g: 65k, 50g: 18k)
- Dâu Sấy Dẻo (200g: 90k, 50g: 25k)
- Hồng Sấy Dẻo (200g: 95k, 50g: 27k)
- Mít Sấy Giòn (200g: 80k, 50g: 22k)
- Chuối Sấy Giòn (200g: 80k, 50g: 22k)
- Dâu Sấy Thăng Hoa (100g: 140k, 50g: 75k)
- Sữa Chua Sấy Thăng Hoa (100g: 95k, 50g: 50k)

CHỈ trả về JSON, không giải thích."""

                response = self.client.chat.completions.create(
                    model="gpt-4o-mini",
                    messages=[
                        {"role": "system", "content": "Bạn là AI chuyên phân tích ý định mua hàng. Chỉ trả về JSON."},
                        {"role": "user", "content": extract_prompt}
                    ],
                    max_tokens=200,
                    temperature=0.1
                )
                
                result_text = response.choices[0].message.content.strip()
                print(f"[DEBUG] Purchase extraction result: {result_text}")
                
                # Parse JSON
                import json
                import re
                json_match = re.search(r'\{.*\}', result_text, re.DOTALL)
                if json_match:
                    result = json.loads(json_match.group(0))
                    print(f"[DEBUG] Parsed purchase intent: {result}")
                    return result
                    
            except Exception as e:
                print(f"[ERROR] Purchase intent extraction error: {e}")
        
        # Fallback: simple detection
        return {
            'is_purchase': has_purchase_intent,
            'products': [],
            'confidence': 0.7 if has_purchase_intent else 0.1
        }

    def generate_response(self, question: str, search_results: List[Dict] = None, conversation_history: List[Dict] = None, purchase_intent: Dict = None) -> str:
        """Generate response - wrapper cho chat method"""
        print(f"[DEBUG] generate_response called with question: {question}")
        print(f"[DEBUG] Purchase intent: {purchase_intent}")
        
        # Tạo context từ search results
        context_text = ""
        if search_results:
            context_text = "\n".join([result.get('content', '') for result in search_results[:3]])
        
        # Xử lý purchase intent
        enhanced_question = question
        if purchase_intent and purchase_intent.get('is_purchase') and purchase_intent.get('products'):
            products = purchase_intent.get('products', [])
            product_info = []
            total_price = 0
            
            for product in products:
                name = product.get('name', '')
                quantity = product.get('quantity', 1)
                size = product.get('size', '200g')
                
                # Tính giá (đơn giản hóa)
                price_map = {
                    'Dâu Sấy Thăng Hoa': {'100g': 140000, '50g': 75000},
                    'Mận Sấy Dẻo': {'200g': 65000, '50g': 18000},
                    'Xoài Sấy Dẻo': {'200g': 70000, '50g': 20000},
                    'Đào Sấy Dẻo': {'200g': 65000, '50g': 18000},
                    'Dâu Sấy Dẻo': {'200g': 90000, '50g': 25000},
                    'Hồng Sấy Dẻo': {'200g': 95000, '50g': 27000},
                    'Mít Sấy Giòn': {'200g': 80000, '50g': 22000},
                    'Chuối Sấy Giòn': {'200g': 80000, '50g': 22000},
                    'Sữa Chua Sấy Thăng Hoa': {'100g': 95000, '50g': 50000}
                }
                
                # Tìm giá
                price = 0
                for key, prices in price_map.items():
                    if key.lower() in name.lower():
                        price = prices.get(size, 0)
                        break
                
                if price > 0:
                    item_total = price * quantity
                    total_price += item_total
                    product_info.append(f"- {name} ({size}) x{quantity}: {item_total:,}đ")
            
            if product_info:
                cart_info = f"""
🛒 THÔNG TIN ĐƠN HÀNG:
{chr(10).join(product_info)}

💰 TỔNG TIỀN: {total_price:,}đ

✅ Tôi đã thêm sản phẩm vào giỏ hàng cho bạn!

📞 Để hoàn tất đặt hàng, vui lòng:
<<<<<<< HEAD
1. Gọi hotline: 0929.161.999
2. Hoặc nhắn tin Zalo: 0929.161.999
=======
1. Gọi hotline: 0912.345.678
2. Hoặc nhắn tin Zalo: 0912.345.678
>>>>>>> 5c0a37643dba4ed53558e01db64b81ab4615dae9
3. Thanh toán khi nhận hàng (COD)

🚚 Miễn phí ship nội thành, giao hàng trong 24h!"""
                
                enhanced_question = f"Khách hàng muốn đặt hàng. Hãy xác nhận đơn hàng này:\n{cart_info}\n\nCâu hỏi gốc: {question}"
        
        # Tạo message với context
        if context_text and not purchase_intent:
            enhanced_question = f"Dựa trên thông tin sau:\n{context_text}\n\nCâu hỏi: {question}"
            
        return self.chat(enhanced_question, conversation_history)

    def get_model_info(self) -> Dict:
        """Thông tin về model đang sử dụng"""
        return {
            "model_type": self.model_type,
            "model_name": "gpt-4o-mini" if self.model_type == "openai" else "gemini-2.0-flash" if self.model_type == "gemini" else "simple",
            "status": "active" if self.model_type != "none" else "fallback"
        }
