function UpdateCart(url) {
    $.ajax({
        url: url,
        type: 'POST', // Or GET, depending on your Controller attribute
        success: function (result) {
            // 'result' here is the HTML for the ViewComponent
            console.log(result)
            $('#cart-container').html(result);
        },
        error: function () {
            alert("Error updating cart. Please try again.");
        }
    });
}
