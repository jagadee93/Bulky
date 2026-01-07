
$(function () {
    var url = window.location.search;

    if (url.includes("pending")) {
        LoadTable("pending")
    } else if (url.includes("completed")) {
        LoadTable("completed")
    } else if (url.includes("approved")) {
        LoadTable("approved")
    } else if (url.includes("inprocess")) {
        LoadTable("inprocess")
    } else {
        LoadTable("all")
    }
    
})





var datatable;





function LoadTable(status) {
    DataTable = $('#OrdersTbl').DataTable({
        ajax: { url: '/Admin/Order/Getall?status=' + status, dataSrc: '' },
        columns: [
            { data: "id", "width": "1%" },
            { data: "shippingAddress.name", "width": "5%" },
            { data: "shippingAddress.phoneNumber", "width": "3%" },
            { data: "applicationUser.email", "width": "6%" },
            { data: "orderTotal", "width": "2%" },
            { data: "paymentDueDate", "width": "4%" },
            { data: "paymentStatus", width: "6%" },
            {
                data: "id", width: "2%",
                render: function (data) {
                    return ` <div class="" role="group">
                 <a href="/Admin/Product/Upsert/${data}"  class="me-3 btn btn-primary" title="Edit" ><i class="bi bi-pencil-square"></i></a>
                
                </div>`;
                }
            }
        ]
    });

}