/**
 * AI Chat Widget
 * Tích hợp với Mộc Châu Fruits AI API
 */

class AIChatWidget {
    constructor() {
        // API Configuration - Will be loaded from server
        this.baseUrl = null;
        this.apiUrl = null;
        this.healthUrl = null;

        this.isOpen = false;
        this.isTyping = false;
        this.messages = [];

        // Generate or load session ID
        this.sessionId = this.getOrCreateSessionId();

        this.init();
    }

    getOrCreateSessionId() {
        let sessionId = localStorage.getItem('aiChatSessionId');
        if (!sessionId) {
            sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('aiChatSessionId', sessionId);
        }
        return sessionId;
    }

    async init() {
        // Load AI API URL from server configuration
        await this.loadAIConfig();

        this.createWidget();
        this.attachEventListeners();
        this.checkAIHealth();

        // Load chat history from localStorage
        this.loadChatHistory();
    }

    async loadAIConfig() {
        try {
            const response = await fetch('/Home/GetAIConfig');
            const config = await response.json();
            this.baseUrl = config.apiUrl;
            this.apiUrl = `${this.baseUrl}/api/chat`;
            this.healthUrl = `${this.baseUrl}/api/health`;
        } catch (error) {
            console.error('Failed to load AI config, using default:', error);
            this.baseUrl = 'http://localhost:8000';
            this.apiUrl = 'http://localhost:8000/api/chat';
            this.healthUrl = 'http://localhost:8000/api/health';
        }
    }

    createWidget() {
        const widgetHTML = `
            <div class="ai-chat-widget">
                <!-- Floating Button -->
                <button class="ai-chat-button" id="aiChatButton" title="Chat với AI">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64" fill="white">
                        <!-- Robot Head -->
                        <rect x="16" y="20" width="32" height="28" rx="4" fill="white"/>
                        <!-- Antenna -->
                        <line x1="32" y1="12" x2="32" y2="20" stroke="white" stroke-width="2"/>
                        <circle cx="32" cy="10" r="3" fill="white"/>
                        <!-- Eyes -->
                        <circle cx="24" cy="30" r="3" fill="#82ae46"/>
                        <circle cx="40" cy="30" r="3" fill="#82ae46"/>
                        <!-- Mouth -->
                        <rect x="22" y="40" width="20" height="4" rx="2" fill="#82ae46"/>
                        <!-- Ears -->
                        <rect x="12" y="26" width="4" height="8" rx="2" fill="white"/>
                        <rect x="48" y="26" width="4" height="8" rx="2" fill="white"/>
                    </svg>
                    <span class="ai-chat-badge" id="aiChatBadge" style="display: none;">1</span>
                </button>
                
                <!-- Chat Window -->
                <div class="ai-chat-window" id="aiChatWindow">
                    <!-- Header -->
                    <div class="ai-chat-header">
                        <div class="ai-chat-header-info">
                            <div class="ai-chat-avatar">🤖</div>
                            <div class="ai-chat-title">
                                <h3>AI Mộc Châu</h3>
                                <div class="ai-chat-status">
                                    <span class="ai-chat-status-dot"></span>
                                    <span id="aiChatStatus">Đang kết nối...</span>
                                </div>
                            </div>
                        </div>
                        <div class="ai-chat-header-actions">
                            <button class="ai-chat-history" id="aiChatHistory" title="Xem lịch sử chat">
                                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <circle cx="12" cy="12" r="10"></circle>
                                    <polyline points="12 6 12 12 16 14"></polyline>
                                </svg>
                            </button>
                            <button class="ai-chat-archive" id="aiChatArchive" title="Lưu trữ và bắt đầu chat mới">
                                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <polyline points="21 8 21 21 3 21 3 8"></polyline>
                                    <rect x="1" y="3" width="22" height="5"></rect>
                                    <line x1="10" y1="12" x2="14" y2="12"></line>
                                </svg>
                            </button>
                            <button class="ai-chat-close" id="aiChatClose" title="Đóng">×</button>
                        </div>
                    </div>
                    
                    <!-- Messages -->
                    <div class="ai-chat-messages" id="aiChatMessages">
                        <div class="ai-welcome-message">
                            <h4>👋 Xin chào!</h4>
                            <p>Tôi là trợ lý AI của Mộc Vị Store. Tôi có thể giúp bạn tìm hiểu về các loại hoa quả sấy Mộc Châu.</p>
                            <div class="ai-quick-questions">
                                <button class="ai-quick-question" data-question="Cho tôi biết về dâu tây sấy">
                                    🍓 Dâu tây sấy có gì đặc biệt?
                                </button>
                                <button class="ai-quick-question" data-question="Các loại hoa quả sấy nào tốt cho sức khỏe?">
                                    💪 Hoa quả nào tốt cho sức khỏe?
                                </button>
                                <button class="ai-quick-question" data-question="Giá của các sản phẩm như thế nào?">
                                    💰 Giá sản phẩm ra sao?
                                </button>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Input -->
                    <div class="ai-chat-input-container">
                        <input 
                            type="text" 
                            class="ai-chat-input" 
                            id="aiChatInput" 
                            placeholder="Hỏi về hoa quả Mộc Châu..."
                            autocomplete="off"
                        />
                        <button class="ai-chat-send" id="aiChatSend" title="Gửi">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z"/>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', widgetHTML);
    }

    attachEventListeners() {
        // Toggle chat window
        document.getElementById('aiChatButton').addEventListener('click', () => {
            this.toggleChat();
        });

        document.getElementById('aiChatClose').addEventListener('click', () => {
            this.toggleChat();
        });

        // Send message
        document.getElementById('aiChatSend').addEventListener('click', () => {
            this.sendMessage();
        });

        document.getElementById('aiChatInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.sendMessage();
            }
        });

        // Quick questions
        document.querySelectorAll('.ai-quick-question').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const question = e.target.getAttribute('data-question');
                this.sendMessage(question);
            });
        });

        // View chat history
        document.getElementById('aiChatHistory').addEventListener('click', () => {
            this.showChatHistory();
        });

        // Archive and start new chat
        document.getElementById('aiChatArchive').addEventListener('click', () => {
            this.archiveAndStartNew();
        });
    }

    toggleChat() {
        this.isOpen = !this.isOpen;
        const chatWindow = document.getElementById('aiChatWindow');
        const chatButton = document.getElementById('aiChatButton');
        const chatBadge = document.getElementById('aiChatBadge');

        if (this.isOpen) {
            chatWindow.classList.add('active');
            chatButton.classList.add('active');
            chatBadge.style.display = 'none';
            document.getElementById('aiChatInput').focus();
        } else {
            chatWindow.classList.remove('active');
            chatButton.classList.remove('active');
        }
    }

    async checkAIHealth() {
        const statusElement = document.getElementById('aiChatStatus');

        try {
            const response = await fetch(this.healthUrl);
            const data = await response.json();

            if (data.status === 'healthy') {
                statusElement.textContent = 'Trực tuyến';
                statusElement.style.color = '#4ade80';
            } else {
                statusElement.textContent = 'Không khả dụng';
                statusElement.style.color = '#fbbf24';
            }
        } catch (error) {
            console.error('AI Health check failed:', error);
            statusElement.textContent = 'Ngoại tuyến';
            statusElement.style.color = '#ef4444';

            // Show notification badge
            document.getElementById('aiChatBadge').style.display = 'flex';
        }
    }

    async sendMessage(customMessage = null) {
        const input = document.getElementById('aiChatInput');
        const message = customMessage || input.value.trim();

        if (!message) return;

        // Clear input
        input.value = '';

        // Add user message
        this.addMessage('user', message);

        // Show typing indicator
        this.showTypingIndicator();

        try {
            // Prepare conversation history (last 8 messages for context, excluding current message)
            // We need to exclude the current user message that was just added
            const conversationHistory = this.messages.slice(-8, -1).map(msg => ({
                role: msg.type === 'user' ? 'user' : 'assistant',
                content: msg.content
            }));

            // Call AI API with conversation history
            const response = await fetch(this.apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    question: message,
                    top_k: 3,
                    conversation_history: conversationHistory
                })
            });

            if (!response.ok) {
                throw new Error('API request failed');
            }

            const data = await response.json();

            // Hide typing indicator
            this.hideTypingIndicator();

            // Add AI response
            this.addMessage('bot', data.answer, data.sources);

            // Handle order action if present
            if (data.action && data.action.type === 'add_to_cart') {
                await this.handleOrderAction(data.action);
            }

        } catch (error) {
            console.error('Error sending message:', error);
            this.hideTypingIndicator();

            // Show error message
            this.addMessage('bot',
                '❌ Xin lỗi, tôi đang gặp sự cố kết nối. Vui lòng đảm bảo AI server đang chạy (python app/main.py) hoặc thử lại sau.',
                []
            );
        }

        // Save chat history
        this.saveChatHistory();
    }

    addMessage(type, content, sources = [], isHTML = false) {
        const messagesContainer = document.getElementById('aiChatMessages');
        const time = new Date().toLocaleTimeString('vi-VN', {
            hour: '2-digit',
            minute: '2-digit'
        });

        // Remove welcome message if exists
        const welcomeMsg = messagesContainer.querySelector('.ai-welcome-message');
        if (welcomeMsg) {
            welcomeMsg.remove();
        }

        const messageHTML = `
            <div class="ai-message ${type}">
                <div class="ai-message-avatar">
                    ${type === 'bot' ? '🤖' : '👤'}
                </div>
                <div class="ai-message-content">
                    <div class="ai-message-bubble">
                        ${isHTML ? content : this.formatMessage(content)}
                    </div>
                    <div class="ai-message-time">${time}</div>
                </div>
            </div>
        `;

        messagesContainer.insertAdjacentHTML('beforeend', messageHTML);

        // Add sources if available
        if (sources && sources.length > 0) {
            const sourcesHTML = `
                <div class="ai-message bot">
                    <div class="ai-message-avatar">📚</div>
                    <div class="ai-message-content">
                        <div class="ai-message-bubble" style="font-size: 12px; background: #f8f9fa; color: #6c757d;">
                            <strong>Nguồn tham khảo:</strong><br/>
                            ${sources.map((s, i) =>
                `${i + 1}. ${s.fruit_name} (${(s.relevance_score * 100).toFixed(0)}%)`
            ).join('<br/>')}
                        </div>
                    </div>
                </div>
            `;
            messagesContainer.insertAdjacentHTML('beforeend', sourcesHTML);
        }

        // Scroll to bottom
        messagesContainer.scrollTop = messagesContainer.scrollHeight;

        // Store message
        this.messages.push({ type, content, time, sources });
    }

    formatMessage(text) {
        // Convert markdown-style formatting
        return text
            .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
            .replace(/\*(.*?)\*/g, '<em>$1</em>')
            .replace(/\n/g, '<br/>');
    }

    showTypingIndicator() {
        const messagesContainer = document.getElementById('aiChatMessages');
        const typingHTML = `
            <div class="ai-message bot" id="aiTypingIndicator">
                <div class="ai-message-avatar">🤖</div>
                <div class="ai-message-content">
                    <div class="ai-typing-indicator">
                        <div class="ai-typing-dot"></div>
                        <div class="ai-typing-dot"></div>
                        <div class="ai-typing-dot"></div>
                    </div>
                </div>
            </div>
        `;

        messagesContainer.insertAdjacentHTML('beforeend', typingHTML);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;

        // Disable input
        document.getElementById('aiChatInput').disabled = true;
        document.getElementById('aiChatSend').disabled = true;
    }

    hideTypingIndicator() {
        const indicator = document.getElementById('aiTypingIndicator');
        if (indicator) {
            indicator.remove();
        }

        // Enable input
        document.getElementById('aiChatInput').disabled = false;
        document.getElementById('aiChatSend').disabled = false;
        document.getElementById('aiChatInput').focus();
    }

    async saveChatToDatabase(role, message, isOrderRelated = false, orderData = null) {
        try {
            await fetch('/api/ChatHistory/save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    sessionId: this.sessionId,
                    role: role,
                    message: message,
                    isOrderRelated: isOrderRelated,
                    orderData: orderData
                })
            });
        } catch (error) {
            console.error('Error saving chat to database:', error);
        }
    }

    saveChatHistory() {
        try {
            localStorage.setItem('aiChatHistory', JSON.stringify(this.messages));
        } catch (error) {
            console.error('Error saving chat history:', error);
        }
    }

    loadChatHistory() {
        try {
            const history = localStorage.getItem('aiChatHistory');
            if (history) {
                this.messages = JSON.parse(history);

                // Restore messages (limit to last 10)
                const recentMessages = this.messages.slice(-10);
                if (recentMessages.length > 0) {
                    const messagesContainer = document.getElementById('aiChatMessages');
                    const welcomeMsg = messagesContainer.querySelector('.ai-welcome-message');
                    if (welcomeMsg) {
                        welcomeMsg.remove();
                    }

                    recentMessages.forEach(msg => {
                        this.addMessage(msg.type, msg.content, msg.sources || []);
                    });
                }
            }
        } catch (error) {
            console.error('Error loading chat history:', error);
        }
    }

    async handleOrderAction(action) {
        try {
            // Show processing message
            this.addMessage('bot', '⏳ Đang xử lý đơn hàng của bạn...');

            // Call order API
            const response = await fetch('/api/AIOrder/add-to-cart', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    products: action.products
                })
            });

            const result = await response.json();

            if (result.success) {
                // Show success message with action buttons
                const messageHTML = `
                    <div style="line-height: 1.6;">
                        <p style="margin-bottom: 15px;">${result.message}</p>
                        <div style="display: flex; gap: 10px; flex-wrap: wrap;">
                            <button onclick="window.location.href='/Cart'" 
                                    style="flex: 1; min-width: 140px; padding: 10px 20px; background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 14px; transition: all 0.3s; box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3);">
                                🛒 Xem giỏ hàng
                            </button>
                            <button onclick="window.location.href='/Cart/Checkout'" 
                                    style="flex: 1; min-width: 140px; padding: 10px 20px; background: linear-gradient(135deg, #007bff 0%, #0056b3 100%); color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 14px; transition: all 0.3s; box-shadow: 0 2px 8px rgba(0, 123, 255, 0.3);">
                                💳 Thanh toán ngay
                            </button>
                        </div>
                    </div>
                `;
                this.addMessage('bot', messageHTML, [], true);
            } else if (result.requiresLogin) {
                // Show login required message
                const messageHTML = `
                    <div style="line-height: 1.6;">
                        <p style="margin-bottom: 15px;">${result.message}</p>
                        <button onclick="window.location.href='${result.redirectUrl}'" 
                                style="width: 100%; padding: 12px 24px; background: linear-gradient(135deg, #007bff 0%, #0056b3 100%); color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 15px; transition: all 0.3s; box-shadow: 0 2px 8px rgba(0, 123, 255, 0.3);">
                            🔐 Đăng nhập để tiếp tục
                        </button>
                    </div>
                `;
                this.addMessage('bot', messageHTML, [], true);
            } else {
                this.addMessage('bot', `❌ ${result.message}`);
            }

        } catch (error) {
            console.error('Error handling order action:', error);
            this.addMessage('bot', '❌ Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại!');
        }
    }

    showChatHistory() {
        if (this.messages.length === 0) {
            alert('📭 Chưa có tin nhắn nào trong cuộc trò chuyện hiện tại!\n\nMẹo: Hãy chat với AI trước, sau đó bấm nút này để xem thống kê.');
            return;
        }

        const totalMessages = this.messages.length;
        const userMessages = this.messages.filter(m => m.type === 'user').length;
        const botMessages = this.messages.filter(m => m.type === 'bot').length;

        const historyHTML = `
            <div style="line-height: 1.8;">
                <h4 style="margin-bottom: 15px; color: #007bff;">📊 Thống Kê Lịch Sử Chat</h4>
                <div style="background: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 15px;">
                    <p style="margin: 5px 0;"><strong>💬 Tổng số tin nhắn:</strong> ${totalMessages}</p>
                    <p style="margin: 5px 0;"><strong>👤 Tin nhắn của bạn:</strong> ${userMessages}</p>
                    <p style="margin: 5px 0;"><strong>🤖 Tin nhắn AI:</strong> ${botMessages}</p>
                </div>
                <p style="color: #6c757d; font-size: 13px;">
                    💡 <strong>Mẹo:</strong> Lịch sử chat được lưu tự động trong trình duyệt của bạn. 
                    Bạn có thể xóa lịch sử bằng nút 🗑️ trên thanh công cụ.
                </p>
                <div style="margin-top: 15px;">
                    <button onclick="document.getElementById('aiChatMessages').scrollTop = 0" 
                            style="width: 100%; padding: 10px; background: linear-gradient(135deg, #6c757d 0%, #495057 100%); color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; transition: all 0.3s; box-shadow: 0 2px 8px rgba(108, 117, 125, 0.3);"
                            onmouseover="this.style.transform='translateY(-2px)'; this.style.boxShadow='0 4px 12px rgba(108, 117, 125, 0.4)'"
                            onmouseout="this.style.transform='translateY(0)'; this.style.boxShadow='0 2px 8px rgba(108, 117, 125, 0.3)'">
                        ⬆️ Cuộn lên đầu chat
                    </button>
                </div>
            </div>
        `;

        this.addMessage('bot', historyHTML, [], true);
    }

    showArchiveHistoryPopup() {
        // Create popup overlay
        const popup = document.createElement('div');
        popup.id = 'archiveHistoryPopup';
        popup.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
        `;

        const totalMessages = this.messages.length;
        const userMessages = this.messages.filter(m => m.type === 'user').length;
        const botMessages = this.messages.filter(m => m.type === 'bot').length;

        popup.innerHTML = `
            <div style="background: white; border-radius: 12px; padding: 24px; max-width: 500px; width: 90%; max-height: 80vh; overflow-y: auto; box-shadow: 0 4px 20px rgba(0,0,0,0.15);">
                <h3 style="margin: 0 0 16px 0; color: #202123; font-size: 20px;">📦 Lưu trữ cuộc trò chuyện</h3>
                
                <div style="background: #f7f7f8; padding: 16px; border-radius: 8px; margin-bottom: 16px;">
                    <p style="margin: 0 0 8px 0; color: #565869; font-size: 14px;"><strong>Thống kê:</strong></p>
                    <p style="margin: 4px 0; color: #202123;">💬 Tổng số tin nhắn: <strong>${totalMessages}</strong></p>
                    <p style="margin: 4px 0; color: #202123;">👤 Tin nhắn của bạn: <strong>${userMessages}</strong></p>
                    <p style="margin: 4px 0; color: #202123;">🤖 Tin nhắn AI: <strong>${botMessages}</strong></p>
                </div>
                
                <div style="background: #fff4e5; padding: 12px; border-radius: 8px; margin-bottom: 20px; border-left: 3px solid #ff9800;">
                    <p style="margin: 0; color: #663c00; font-size: 13px;">
                        ⚠️ Cuộc trò chuyện này sẽ được lưu vào hệ thống và bạn sẽ bắt đầu một cuộc trò chuyện mới.
                    </p>
                </div>
                
                <div style="display: flex; gap: 12px; justify-content: flex-end;">
                    <button id="cancelArchive" style="padding: 10px 20px; background: #f7f7f8; color: #202123; border: none; border-radius: 6px; cursor: pointer; font-weight: 600; font-size: 14px;">
                        Hủy
                    </button>
                    <button id="confirmArchive" style="padding: 10px 20px; background: #10a37f; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600; font-size: 14px;">
                        Lưu trữ
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(popup);

        // Event listeners
        document.getElementById('cancelArchive').addEventListener('click', () => {
            popup.remove();
        });

        document.getElementById('confirmArchive').addEventListener('click', async () => {
            popup.remove();
            await this.performArchive();
        });

        // Close on overlay click
        popup.addEventListener('click', (e) => {
            if (e.target === popup) {
                popup.remove();
            }
        });
    }

    async performArchive() {
        try {
            // Show archiving message
            const archiveMessage = `
                <div style="text-align: center; padding: 20px; background: #e3f2fd; border-radius: 8px; margin: 10px 0;">
                    <p style="margin: 0; color: #1976d2; font-weight: 600;">
                        📦 Đang lưu trữ ${this.messages.length} tin nhắn...
                    </p>
                </div>
            `;
            const messagesContainer = document.getElementById('aiChatMessages');
            messagesContainer.insertAdjacentHTML('beforeend', archiveMessage);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;

            // Save all messages to database at once
            for (const msg of this.messages) {
                const role = msg.type === 'user' ? 'user' : 'assistant';
                await this.saveChatToDatabase(role, msg.content, false, null);
            }

            // Wait a bit for visual feedback
            await new Promise(resolve => setTimeout(resolve, 500));

            // Clear messages array
            this.messages = [];

            // Clear localStorage
            localStorage.removeItem('aiChatHistory');

            // Generate new session ID
            const oldSessionId = this.sessionId;
            localStorage.removeItem('aiChatSessionId');
            this.sessionId = this.getOrCreateSessionId();

            // Clear UI and show welcome message
            messagesContainer.innerHTML = `
                <div class="ai-welcome-message">
                    <h4>✨ Cuộc trò chuyện mới!</h4>
                    <p style="color: #28a745; font-weight: 600;">
                        ✅ Đã lưu trữ cuộc trò chuyện trước (Session: ${oldSessionId.substring(0, 20)}...)
                    </p>
                    <p>Tôi là trợ lý AI của Mộc Vị Store. Tôi có thể giúp bạn tìm hiểu về các loại hoa quả sấy Mộc Châu.</p>
                    <div class="ai-quick-questions">
                        <button class="ai-quick-question" data-question="Cho tôi biết về dâu tây sấy">
                            🍓 Dâu tây sấy có gì đặc biệt?
                        </button>
                        <button class="ai-quick-question" data-question="Các loại hoa quả sấy nào tốt cho sức khỏe?">
                            💪 Hoa quả nào tốt cho sức khỏe?
                        </button>
                        <button class="ai-quick-question" data-question="Giá của các sản phẩm như thế nào?">
                            💰 Giá sản phẩm ra sao?
                        </button>
                    </div>
                </div>
            `;

            // Re-attach quick question listeners
            document.querySelectorAll('.ai-quick-question').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const question = e.target.getAttribute('data-question');
                    this.sendMessage(question);
                });
            });

            console.log(`Chat archived! Old session: ${oldSessionId}, New session: ${this.sessionId}`);
        } catch (error) {
            console.error('Error archiving chat:', error);
            alert('❌ Có lỗi khi lưu trữ chat. Vui lòng thử lại!');
        }
    }

    async archiveAndStartNew() {
        if (this.messages.length === 0) {
            alert('📭 Chưa có tin nhắn nào để lưu trữ!');
            return;
        }

        // Show archive history popup
        this.showArchiveHistoryPopup();
    }
}

// Initialize widget when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        new AIChatWidget();
    });
} else {
    new AIChatWidget();
}
