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
            messages = [
                {"role": "system", "content": "Bạn là AI assistant thân thiện của Mộc Vị Store - cửa hàng hoa quả sấy cao cấp từ Mộc Châu. Hãy trả lời một cách tự nhiên và hữu ích."},
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
        """Phát hiện ý định mua hàng"""
        print(f"[DEBUG] Detecting purchase intent for: {query}")
        return {'is_purchase': False, 'products': [], 'confidence': 0.1}

    def get_model_info(self) -> Dict:
        """Thông tin về model đang sử dụng"""
        return {
            "model_type": self.model_type,
            "model_name": "gpt-4o-mini" if self.model_type == "openai" else "gemini-2.0-flash" if self.model_type == "gemini" else "simple",
            "status": "active" if self.model_type != "none" else "fallback"
        }
