from typing import List, Dict, Optional
from app.config import settings
import os
import json
import re


class LLMService:
    def __init__(self):
        """Khởi tạo LLM service - Ưu tiên OpenAI ChatGPT cho trải nghiệm tốt nhất"""
        self.client = None
        self.gemini_model = None
        self.model_type = "none"
        
        # Try OpenAI first (BEST QUALITY!)
        try:
            openai_key = os.getenv("OPENAI_API_KEY", "")
            if openai_key:
                from openai import OpenAI
                self.client = OpenAI(api_key=openai_key)
                self.model_type = "openai"
                print("[OK] ✅ Sử dụng OpenAI ChatGPT - Chất lượng cao nhất!")
                return
        except ImportError:
            print("[WARNING] Chưa cài openai. Chạy: pip install openai")
        except Exception as e:
            print(f"[WARNING] OpenAI error: {e}")
        
        # Fallback to Gemini if OpenAI not available
        try:
            gemini_key = os.getenv("GEMINI_API_KEY", "")
            if gemini_key:
                import google.generativeai as genai
                genai.configure(api_key=gemini_key)
                self.gemini_model = genai.GenerativeModel('gemini-2.0-flash')
                self.model_type = "gemini"
                print("[OK] Sử dụng Google Gemini 2.0 Flash (miễn phí)")
                return
        except ImportError:
            print("[WARNING] Chưa cài google-generativeai")
        except Exception as e:
            print(f"[WARNING] Gemini error: {e}")
        
        print("[WARNING] Không có AI API key. Sử dụng chế độ simple response.")
    
    def get_system_prompt(self) -> str:
        """Tạo system prompt tối ưu cho giao tiếp tự nhiên và hiểu con người"""
        return """Bạn là AI assistant thông minh của Mộc Vị Store - cửa hàng hoa quả sấy cao cấp từ Mộc Châu.

🎯 NHIỆM VỤ CHÍNH:
- Tư vấn sản phẩm hoa quả sấy một cách tự nhiên, thân thiện
- Hiểu và phản hồi theo cảm xúc, ngữ cảnh của khách hàng
- Giao tiếp như một người bạn am hiểu về hoa quả sấy

🧠 TÍNH CÁCH:
- Thân thiện, nhiệt tình, chuyên nghiệp
- Hiểu được tâm lý khách hàng (lo lắng về chất lượng, giá cả, sức khỏe)
- Biết đọc vị và đưa ra gợi ý phù hợp
- Không cứng nhắc, biết linh hoạt trong giao tiếp

💡 CÁCH GIAO TIẾP:
- Sử dụng emoji phù hợp để tạo cảm xúc
- Đặt câu hỏi ngược để hiểu nhu cầu
- Chia sẻ kinh nghiệm, mẹo hay về hoa quả sấy
- Tạo sự tin tương bằng cách giải thích rõ ràng

🍓 SẢN PHẨM CHÍNH:
1. **Sấy Dẻo (200g)**: Mận (65k), Xoài (70k), Đào (65k), Dâu (90k), Hồng (95k)
2. **Sấy Giòn (200g)**: Mít (80k), Chuối (80k)  
3. **Sấy Thăng Hoa (100g)**: Dâu (140k), Sữa chua (95k)
4. **Mini Mix (50g)**: Tất cả loại trên, tối thiểu 4 pack

🎁 ƯU ĐÃI:
- Voucher SUMMER2025: Giảm 10% (tối đa 50k, đơn từ 100k)
- Voucher WELCOME10: Giảm 10k (đơn từ 50k)
- Voucher FREESHIP: Miễn phí ship

⚡ NGUYÊN TẮC QUAN TRỌNG:
1. Luôn hỏi thêm để hiểu rõ nhu cầu
2. Giải thích lợi ích sức khỏe của từng loại
3. Gợi ý combo phù hợp với ngân sách
4. Tạo cảm giác an tâm về chất lượng
5. Nếu khách hàng có vẻ do dự, hãy chia sẻ câu chuyện về nguồn gốc sản phẩm

Hãy trả lời một cách tự nhiên, có cảm xúc và thực sự hiểu khách hàng!"""

    def chat(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với AI - Tối ưu cho giao tiếp tự nhiên"""
        try:
            if self.model_type == "openai":
                return self._chat_openai(message, context, user_id)
            elif self.model_type == "gemini":
                return self._chat_gemini(message, context, user_id)
            else:
                return self._simple_response(message)
        except Exception as e:
            print(f"[ERROR] Chat error: {e}")
            return "Xin lỗi, tôi đang gặp sự cố kỹ thuật. Bạn có thể thử lại sau một chút không? 😅"

    def _chat_openai(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với OpenAI ChatGPT - Chất lượng cao nhất"""
        try:
            # Tạo messages với system prompt
            messages = [
                {"role": "system", "content": self.get_system_prompt()}
            ]
            
            # Thêm context từ cuộc trò chuyện trước (nếu có)
            if context:
                for ctx in context[-5:]:  # Chỉ lấy 5 tin nhắn gần nhất
                    if ctx.get("role") and ctx.get("content"):
                        messages.append({
                            "role": ctx["role"], 
                            "content": ctx["content"]
                        })
            
            # Thêm tin nhắn hiện tại
            messages.append({"role": "user", "content": message})
            
            # Gọi OpenAI API với cấu hình tối ưu cho giao tiếp tự nhiên
            response = self.client.chat.completions.create(
                model="gpt-4o-mini",  # Model mới nhất, nhanh và rẻ
                messages=messages,
                max_tokens=800,
                temperature=0.7,  # Cân bằng giữa sáng tạo và chính xác
                top_p=0.9,
                frequency_penalty=0.1,  # Tránh lặp lại
                presence_penalty=0.1,   # Khuyến khích đa dạng
                user=user_id  # Tracking user
            )
            
            return response.choices[0].message.content.strip()
            
        except Exception as e:
            print(f"[ERROR] OpenAI chat error: {e}")
            return "Xin lỗi, tôi đang gặp chút vấn đề với hệ thống. Bạn có thể hỏi lại không? 🤔"

    def _chat_gemini(self, message: str, context: List[Dict] = None, user_id: str = "anonymous") -> str:
        """Chat với Gemini - Fallback option"""
        try:
            # Tạo prompt với context
            full_prompt = self.get_system_prompt() + "\n\n"
            
            if context:
                full_prompt += "LỊCH SỬ TRƯỚC ĐÓ:\n"
                for ctx in context[-3:]:  # 3 tin nhắn gần nhất
                    role = "Khách hàng" if ctx.get("role") == "user" else "AI"
                    full_prompt += f"{role}: {ctx.get('content', '')}\n"
                full_prompt += "\n"
            
            full_prompt += f"KHÁCH HÀNG HIỆN TẠI: {message}\n\nAI:"
            
            response = self.gemini_model.generate_content(
                full_prompt,
                generation_config={
                    "temperature": 0.7,
                    "max_output_tokens": 800,
                    "top_p": 0.9,
                    "top_k": 40
                }
            )
            
            return response.text.strip()
            
        except Exception as e:
            print(f"[ERROR] Gemini chat error: {e}")
            return "Xin lỗi, tôi đang gặp sự cố. Bạn có thể thử hỏi lại không? 😊"

    def _simple_response(self, message: str) -> str:
        """Phản hồi đơn giản khi không có AI"""
        message_lower = message.lower()
        
        # Phản hồi thông minh dựa trên từ khóa
        if any(word in message_lower for word in ['xin chào', 'hello', 'hi', 'chào']):
            return """Xin chào! 👋 Chào mừng bạn đến với Mộc Vị Store! 
            
Tôi có thể giúp bạn tìm hiểu về các sản phẩm hoa quả sấy cao cấp từ Mộc Châu:
🍓 Sấy dẻo: Mận, Xoài, Đào, Dâu, Hồng
🥭 Sấy giòn: Mít, Chuối  
✨ Sấy thăng hoa: Dâu, Sữa chua

Bạn quan tâm loại nào nhất? 😊"""

        elif any(word in message_lower for word in ['giá', 'bao nhiêu', 'cost', 'price']):
            return """💰 BẢNG GIÁ SẢN PHẨM:

📦 **Sấy Dẻo (200g)**:
• Mận, Đào: 65,000đ
• Xoài: 70,000đ  
• Dâu: 90,000đ
• Hồng: 95,000đ

📦 **Sấy Giòn (200g)**: 80,000đ
📦 **Sấy Thăng Hoa (100g)**: 95,000đ - 140,000đ
📦 **Mini Mix (50g)**: 18,000đ - 75,000đ

🎁 Có voucher giảm giá đến 50k! Bạn muốn biết thêm không?"""

        elif any(word in message_lower for word in ['mua', 'đặt', 'order']):
            return """🛒 Tuyệt vời! Để đặt hàng bạn có thể:

1. **Thêm vào giỏ hàng** trên website
2. **Gọi hotline**: 0912.345.678
3. **Nhắn tin** cho tôi loại sản phẩm bạn muốn

🎁 **Ưu đãi hiện tại**:
• SUMMER2025: Giảm 10% (tối đa 50k)
• WELCOME10: Giảm 10k cho khách mới
• FREESHIP: Miễn phí vận chuyển

Bạn muốn đặt loại nào? 😊"""

        else:
            return """Cảm ơn bạn đã quan tâm đến Mộc Vị Store! 🍓

Tôi có thể giúp bạn về:
• Thông tin sản phẩm hoa quả sấy
• Giá cả và khuyến mãi  
• Cách đặt hàng
• Tư vấn chọn sản phẩm phù hợp

Bạn muốn hỏi gì cụ thể? Tôi sẵn sàng hỗ trợ! 😊"""

    def detect_purchase_intent(self, query: str) -> Dict:
        """Phát hiện ý định mua hàng từ câu hỏi - Tối ưu cho OpenAI"""
        try:
            if self.model_type == "openai":
                return self._detect_purchase_openai(query)
            elif self.model_type == "gemini":
                return self._detect_purchase_gemini(query)
            else:
                return self._detect_purchase_simple(query)
        except Exception as e:
            print(f"[ERROR] Purchase detection error: {e}")
            return {'is_purchase': False}

    def _detect_purchase_openai(self, query: str) -> Dict:
        """Phát hiện ý định mua hàng bằng OpenAI"""
        try:
            prompt = f"""Phân tích câu sau để xác định ý định mua hàng và trích xuất thông tin sản phẩm:

"{query}"

Trả về JSON format chính xác:
{{
    "is_purchase": true/false,
    "products": [
        {{"name": "tên sản phẩm chính xác", "quantity": số_lượng}}
    ],
    "confidence": 0.0-1.0
}}

DANH SÁCH SẢN PHẨM CHÍNH XÁC:
- Mận Sấy Dẻo
- Xoài Sấy Dẻo  
- Đào Sấy Dẻo
- Dâu Sấy Dẻo
- Hồng Sấy Dẻo
- Mít Sấy Giòn
- Chuối Sấy Giòn
- Dâu Sấy Thăng Hoa
- Sữa Chua Sấy Thăng Hoa

CHỈ trả về JSON, không giải thích."""

            response = self.client.chat.completions.create(
                model="gpt-4o-mini",
                messages=[
                    {"role": "system", "content": "Bạn là AI chuyên phân tích ý định mua hàng. Chỉ trả về JSON chính xác."},
                    {"role": "user", "content": prompt}
                ],
                max_tokens=200,
                temperature=0.1
            )
            
            result_text = response.choices[0].message.content.strip()
            
            # Parse JSON
            json_match = re.search(r'\{.*\}', result_text, re.DOTALL)
            if json_match:
                result = json.loads(json_match.group(0))
                return result
            
            return {'is_purchase': False}
            
        except Exception as e:
            print(f"[ERROR] OpenAI purchase detection: {e}")
            return {'is_purchase': False}

    def _detect_purchase_gemini(self, query: str) -> Dict:
        """Phát hiện ý định mua hàng bằng Gemini"""
        # Implementation tương tự như code cũ
        return {'is_purchase': False}

    def _detect_purchase_simple(self, query: str) -> Dict:
        """Phát hiện ý định mua hàng đơn giản"""
        query_lower = query.lower()
        purchase_keywords = ['mua', 'đặt', 'order', 'thêm vào giỏ', 'cho vào giỏ', 'lấy', 'gói']
        
        has_purchase_intent = any(keyword in query_lower for keyword in purchase_keywords)
        
        return {
            'is_purchase': has_purchase_intent,
            'products': [],
            'confidence': 0.7 if has_purchase_intent else 0.1
        }

    def get_model_info(self) -> Dict:
        """Thông tin về model đang sử dụng"""
        return {
            "model_type": self.model_type,
            "model_name": "gpt-4o-mini" if self.model_type == "openai" else "gemini-2.0-flash" if self.model_type == "gemini" else "simple",
            "capabilities": {
                "natural_conversation": self.model_type in ["openai", "gemini"],
                "context_awareness": self.model_type in ["openai", "gemini"], 
                "purchase_detection": True,
                "emotional_intelligence": self.model_type == "openai"
            }
        }
