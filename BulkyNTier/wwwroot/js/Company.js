$(function () {
    LoadTable();
})


var datatable;





function LoadTable() {
    console.log("Loading Data...")
    datatable = $('#companytbl').DataTable({
        ajax: { url: '/Admin/Company/getall', dataSrc: '' },
        columns: [
            { data: "name", "width": "12%" },
            { data: "streetAddress", "width": "5%" },
            { data: "city", "width": "3%" },
            { data: "state", width: "6%" },
            { data: "postalCode", "width": "5%" },
            { data: "phoneNumber", "width": "5%" },
            {
                data: "id", width: "4%",
                render: function (data) {
                    let updateBtn = ` <a href="/Admin/Company/Upsert/${data}"  class="me-3 btn btn-primary" title="Edit" ><i class="bi bi-pencil-square"></i></a>`;
                    let deleteBtn = `<button data-id="${data}"  class="company-delete-btn btn btn-danger"><i class="bi bi-x-square"></i></button>`;

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
    "click", ".company-delete-btn", function () {
        var id = $(this).data("id");
        deleteCompany(`/Admin/Company/delete/${id}`)
    }
)



function deleteCompany(url) {
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