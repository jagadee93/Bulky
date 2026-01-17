function ShowCartEmpty() {
    let cart = $("#cart-table .cart-item");
    console.log(cart);
    if (cart.length === 0) {
        $("#cart-table").hide();
        $("#empty-cart").removeClass("d-none");
        $("#PriceDetails").hide();
        $("#shipping-address").hide();
        $("#checkout-data").hide();
    }
}


$(document).ready(ShowCartEmpty);


function UpdateCart(url) {
    $.ajax({
        url: url,
        type: 'POST',
        success: function (result) {
            // 'result' here is the HTML for the ViewComponent
            console.log(result)
            $('#cart-container').html(result);
            $("#ShoppingCartCount").load("/Customer/Cart/GetShoppingCartCount");
            $("#PriceDetails").load("/Customer/Cart/GetPriceDetailsComponent");
            ShowCartEmpty();
        },
        error: function () {
            alert("Error updating cart. Please try again.");
        }
    });
}




$("#saveAddressBtn").on("click", function (e) {
    e.preventDefault()
    // Collect form data into a JSON object
    const addressData = {
        // Check if these IDs match your HTML (e.g., id="NewShippingAddress_Name")
        Name: $("#NewShippingAddress_Name").val(),
        PhoneNumber: $("#NewShippingAddress_PhoneNumber").val(),
        StreetAddress: $("#NewShippingAddress_StreetAddress").val(),
        City: $("#NewShippingAddress_City").val(),
        State: $("#NewShippingAddress_State").val(),
        PostalCode: $("#NewShippingAddress_PostalCode").val(),
        AddressType: $("input[name='NewShippingAddress.AddressType']:checked").val(),
        IsDefaultAddress: $("#NewShippingAddress_IsDefaultAddress").is(':checked'),
        AlternatePhoneNumber: $("#NewShippingAddress_AlternatePhoneNumber").val() || null,
        Country: "India" // Required field check
    };
    console.log(addressData);

    $.ajax({
        url: '/Customer/Cart/AddAddress',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(addressData),
        success: function (response) {
            console.log("Executing Funct");
            if (response.success) {
                console.log("Respone is success");
                // 1. Refresh the Address List Section
                $("#addressListContainer").load("/Customer/Cart/GetAddressList", function () {
                    // 2. Hide form, show summary
                    $("#addAddressForm").collapse('hide');

                    // 3. Optional: Automatically trigger "Deliver Here" for the new address
                    // (You can find the first deliver-btn in the newly loaded list)
                    $("#addressListContainer .deliver-btn").first().click();
                });
            }
        }
    });
});
$(document).on("click", ".deliver-btn", function () {
    console.log("Selecting Address ID:", $(this).data("id"));

    // 1. Fill the "Selected Address" display
    $("#selName").text($(this).data("name"));
    $("#selType").text($(this).data("type"));
    $("#selFullAddress").text($(this).data("full"));

    // 2. Update the hidden input value
    // We use the attribute selector to hit the one the Razor Model uses
    $("input[name='SelectedShippingAddressId']").val($(this).data("id"));

    // 3. UI Transitions
    $("#addressListSection").addClass("d-none");
    $("#selectedAddressSection").removeClass("d-none");

    // 4. Force show the Order Summary if it was hidden
    $("#orderSummary").collapse('show');
});

// Fix for the Change Button
$("#changeAddressBtn").on("click", function () {
    // Show the list section again
    $("#addressListSection").removeClass("d-none");

    // Hide the 'Selected' summary section
    $("#selectedAddressSection").addClass("d-none");

    // Optional: Hide the order summary if you want to force them to select again
    $("#orderSummary").collapse('hide');
});
