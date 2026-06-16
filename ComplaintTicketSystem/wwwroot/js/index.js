let gridApi;
let userRole = "";

document.addEventListener('DOMContentLoaded', function () {

    const columnDefs = [
        { field: "complaintId", headerName: "ID", width: 90 },
        { field: "userName", headerName: "User" },
        { field: "categoryName", headerName: "Category" },
        { field: "subject", headerName: "Subject" },
        { field: "description", headerName: "Description" },
        { field: "status", headerName: "Status" },
        { field: "assignedToName", headerName: "Assigned To" },
        { field: "createdDate", headerName: "Created Date" },
        { field: "resolvedDate", headerName: "Resolved Date" },

        {
            headerName: "Actions",
            field: "actions",
            width: 450,
            pinned: "right",
            sortable: false,
            filter: false,
            cellRenderer: actionRenderer
        }
    ];

    const gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            floatingFilter: true
            
        },
        pagination: true
    };

    const gridDiv = document.querySelector('#myGrid');
    gridApi = agGrid.createGrid(gridDiv, gridOptions);

    loadComplaints();
});

function loadComplaints() {
    $.ajax({
        url: "/Complaint/GetComplaints",
        type: "GET",
        success: function (response) {
    console.log(response);
    userRole = response.role;
    let data = response.data;
    const statusFilter =document.getElementById("statusFilter")?.value;
    const categoryFilter =document.getElementById("categoryFilter")?.value;
    if (statusFilter) {
        data = data.filter(x => x.status === statusFilter);
    }
    if (categoryFilter) {
        data = data.filter(x => x.categoryName === categoryFilter);
    }
    gridApi.setGridOption("rowData", data);
},
        error: function (err) {
            console.log("Error loading complaints", err);
        }
    });
}

function actionRenderer(params) {

    let row = params.data;
    let html = "";

    // USER
    if (userRole === "User") {

        html += `
            <button class="btn btn-info btn-sm me-1"
                onclick="viewDetails(${row.complaintId})">
                <i class="bi bi-eye"></i> Details
            </button>
        `;

        if (row.status === "Open") {

            html += `
                <button class="btn btn-warning btn-sm me-1"
                    onclick="editComplaint(${row.complaintId})">
                    <i class="bi bi-pencil-square"></i> Edit
                </button>

                <button class="btn btn-danger btn-sm"
                    onclick="deleteComplaint(${row.complaintId})">
                    <i class="bi bi-trash"></i> Delete
                </button>
            `;
        }
    }

    // SUPPORT
    else if (userRole === "Support") {

        html += `
            <button class="btn btn-info btn-sm me-1"
                onclick="viewDetails(${row.complaintId})">
                <i class="bi bi-eye"></i> Details
            </button>

            <button class="btn btn-success btn-sm"
                onclick="updateStatus(${row.complaintId})">
                <i class="bi bi-arrow-repeat"></i> Update Status
            </button>
        `;
    }

    // ADMIN
    else if (userRole === "Admin") {

        html += `
            <button class="btn btn-info btn-sm me-1"
                onclick="viewDetails(${row.complaintId})">
                <i class="bi bi-eye"></i> Details
            </button>

            <button class="btn btn-success btn-sm me-1"
                onclick="updateStatus(${row.complaintId})">
                <i class="bi bi-arrow-repeat"></i> Update Status
            </button>
        `;

        // Assign button only if not assigned and not resolved
        if (
            (row.assignedTo == null || row.assignedTo === 0) &&
            row.status !== "Resolved"
        ) {

            html += `
                <button class="btn btn-primary btn-sm"
                    onclick="assignComplaint(${row.complaintId})">
                    <i class="bi bi-person-plus"></i> Assign Complaint
                </button>
            `;
        }
    }

    return html;
}

function viewDetails(id) {
    window.location.href = `/Complaint/Details?id=${id}`;
}

function editComplaint(id) {
    window.location.href = `/Complaint/Edit?id=${id}`;
}

function updateStatus(id) {
    window.location.href = `/Admin/Status/${id}`;
}

function assignComplaint(id) {
    window.location.href = `/Admin/Assign/${id}`;
}

function deleteComplaint(id) {

    if (!confirm("Are you sure you want to delete this complaint?"))
        return;

    $.ajax({
        url: "/Complaint/Delete",
        type: "POST",
        data: {
            id: id
        },

        success: function (response) {

            alert(response.message);

            if (response.success) {
                loadComplaints();
            }
        },

        error: function () {
            alert("Delete failed.");
        }
    });
}