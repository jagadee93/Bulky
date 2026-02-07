/// <reference path="company.js" />


$(function () {
    LoadTable();
})

var datatable;





function LoadTable() {
    datatable = $('#tblData').DataTable({
        ajax: { url: '/Admin/Product/getall', dataSrc: '' },
        columns: [
            { data: "title", "width": "12%" },
            { data: "isbn", "width": "3%" },
            { data: "price", "width": "1%" },
            { data: "author", width: "4%" },
            { data: "category.name", "width": "5%" },
            {
                data: "id", width: "3%",
                render: function (data) {


                    let updateBtn = ` <a href="/Admin/Product/Upsert/${data}"  class="me-3 btn btn-primary" title="Edit" ><i class="bi bi-pencil-square"></i></a>`;
                    let deleteBtn = `<button data-id="${data}" title="delete"  class="product-delete-btn btn btn-danger"><i class="bi bi-x-square"></i></button>`;

                    return `
                        <div class="d-flex justify-content-center gap-2">
                            ${updateBtn}
                            ${deleteBtn}
                        </div>
                        `;
                }
            }
        ]
    });
}


$(document).on(
    "click", ".product-delete-btn", function () {
        let productId = $(this).data("id");
        console.log(productId);
        deleteProduct(`/Admin/Product/Delete/${productId}`);
    }
)




function deleteProduct(url) {
    console.log("Deleting");
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: "DELETE",

                success: function (data) {
                    datatable.ajax.reload();
                    if (data.success) {
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}