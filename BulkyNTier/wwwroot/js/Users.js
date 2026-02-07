$(function () {
    LoadTable();
})


var datatable;





function LoadTable() {
    console.log("Loading Data...")
    datatable = $('#usertbl').DataTable({
        ajax: { url: '/Admin/User/getall', dataSrc: '' },
        columns: [
            { data: "name", "width": "12%" },
            { data: "email", "width": "10%" },
            { data: "phoneNumber", "width": "3%" },

            { data: "emailConfirmed", width: "6%" },
            { data: "companyName", "width": "5%" },
            { data: "companyPhoneNumber", "width": "5%" },
            { data: "role", "width": "5%" },
            {
                data: null,
                width: "7%",
                render: function (data, type, row) {

                    const lockBtn = row.lockoutEnabled
                        ? `
                        <button class="btn btn-sm btn-danger btn-lock" data-id="${row.id}" title="Lock User">
                            <i class="bi bi-lock-fill"></i>
                        </button>`
                        : `
                        <button class="btn btn-sm btn-success btn-lock" data-id="${row.id}" title="Unlock User">
                            <i class="bi bi-unlock-fill"></i>
                        </button>`;

                    const permissionBtn = `
                        <a href="/Admin/User/RoleManagement?userId=${row.id}" class="btn btn-sm btn-outline-secondary btn-permission"
                                title="Manage Permissions">
                            <i class="bi bi-pencil-square"></i>
                        </a>`;

                    return `
                        <div class="d-flex justify-content-center gap-2">
                            ${lockBtn}
                            ${permissionBtn}
                        </div>
                        `;
                }
            }
            
        ]
    });
}


$(document).on(
    "click", ".btn-lock", function () {
        let userId = $(this).data("id");
        SetLockout(userId);
    }
)




function SetLockout(userId) {
    $.ajax({
        url: "/Admin/User/SetLockout",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(userId),
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
