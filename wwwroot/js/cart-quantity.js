/**
 * Cart Quantity Management
 * Auto-update cart when quantity changes
 */

$(document).ready(function() {
    // Quantity increase button
    $('.quantity-increase').click(function() {
        const input = $(this).siblings('input[name="quantity"]');
        const currentVal = parseInt(input.val()) || 1;
        const max = parseInt(input.attr('max')) || 999;
        
        if (currentVal < max) {
            const newVal = currentVal + 1;
            input.val(newVal);
            updateCartItem(input);
        }
    });
    
    // Quantity decrease button
    $('.quantity-decrease').click(function() {
        const input = $(this).siblings('input[name="quantity"]');
        const currentVal = parseInt(input.val()) || 1;
        
        if (currentVal > 1) {
            const newVal = currentVal - 1;
            input.val(newVal);
            updateCartItem(input);
        }
    });
    
    // Manual quantity input
    $('input[name="quantity"]').on('change', function() {
        const val = parseInt($(this).val()) || 1;
        const max = parseInt($(this).attr('max')) || 999;
        
        if (val < 1) {
            $(this).val(1);
        } else if (val > max) {
            $(this).val(max);
        }
        
        updateCartItem($(this));
    });
    
    // Update cart item via AJAX
    function updateCartItem(input) {
        const cartId = input.data('cart-id');
        const quantity = parseInt(input.val());
        const row = input.closest('tr');
        
        // Show loading
        row.find('.item-total').html('<i class="fa fa-spinner fa-spin"></i>');
        
        $.ajax({
            url: '/Cart/UpdateQuantity',
            type: 'POST',
            data: {
                cartId: cartId,
                quantity: quantity
            },
            success: function(response) {
                if (response.success) {
                    // Update item total
                    row.find('.item-total').text(formatCurrency(response.itemTotal));
                    
                    // Update cart summary
                    updateCartSummary();
                } else {
                    alert(response.message || 'Có lỗi xảy ra!');
                    // Revert to old value
                    input.val(response.oldQuantity || 1);
                }
            },
            error: function() {
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    }
    
    // Update cart summary (subtotal, discount, total)
    function updateCartSummary() {
        $.ajax({
            url: '/Cart/GetCartSummary',
            type: 'GET',
            success: function(data) {
                $('#cart-subtotal').text(formatCurrency(data.subtotal));
                $('#cart-discount').text(formatCurrency(data.discount));
                $('#cart-total').text(formatCurrency(data.total));
                
                // Update cart count in header
                $('.cart-count').text(data.itemCount);
            }
        });
    }
    
    // Format currency
    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }
    
    // Remove item from cart
    $('.btn-remove-cart-item').click(function(e) {
        e.preventDefault();
        
        if (!confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
            return;
        }
        
        const cartId = $(this).data('cart-id');
        const row = $(this).closest('tr');
        
        $.ajax({
            url: '/Cart/RemoveFromCart',
            type: 'POST',
            data: { cartId: cartId },
            success: function(response) {
                if (response.success) {
                    // Remove row with animation
                    row.fadeOut(300, function() {
                        $(this).remove();
                        
                        // Check if cart is empty
                        if ($('tbody tr').length === 0) {
                            location.reload();
                        } else {
                            updateCartSummary();
                        }
                    });
                } else {
                    alert(response.message || 'Có lỗi xảy ra!');
                }
            },
            error: function() {
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    });
});
