
//import toastr from ""


$(function () {
    LoadTable();
})

var datatable;




function LoadTable() {
    console.log("Loading Data...")
    datatable=$('#tblData').DataTable({
        ajax: { url: '/Admin/Product/getall', dataSrc: '' },
        columns: [
            { data: "title", "width": "12%" },
            { data: "isbn", "width": "5%" },
            { data: "price", "width": "3%" },
            { data: "author", width: "6%" },
            { data: "category.name", "width": "5%" },
            {
                data: "id", width: "7%",
                render: function (data) {
                    return ` <div class="" role="group">
                 <a href="/Admin/Product/Upsert/${data}"  class="me-3 btn btn-primary" title="Edit" ><i class="bi bi-pencil-square"></i>Edit</a>
                 <a onClick=deleteProduct('/Admin/Product/Delete/${data}')  class="btn btn-danger"><i class="bi bi-x-square"></i>Deletee</a>
                </div>`;
                }
            }



        ]
    });
}




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
                    toastr.success(data.message);
                }
            })
        }
    });
}