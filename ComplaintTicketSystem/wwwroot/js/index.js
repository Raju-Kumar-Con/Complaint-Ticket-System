let gridApi;
let userRole = "";

document.addEventListener('DOMContentLoaded', function () {

    const columnDefs = [

        { field: "complaintId", headerName: "ID" },
        { field: "userName", headerName: "User" },
        { field: "categoryName", headerName: "Category" },
        { field: "subject", headerName: "Subject" },
        { field: "description", headerName: "Description" },
        { field: "status", headerName: "Status" },
        { field: "assignedToName", headerName: "Assigned To" },
        {
            field: "createdDate",
            headerName: "Created Date",
            filter: "agDateColumnFilter",
            floatingFilter: true,
            valueFormatter: params => formatDate(params.value)
        },
        {
            field: "resolvedDate",
            headerName: "Resolved Date",
             filter: "agDateColumnFilter",
            floatingFilter: true,
            valueFormatter: params => formatDate(params.value)
        },
        {
            headerName: "Actions",
            field: "actions",
            
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
            flex: 1,
            filter: true,
            resizable: true,
            floatingFilter: true
        },
        rowClassRules: {
            'row-open': params =>params.data?.status === 'Open',
            'row-progress': params =>params.data?.status === 'In Progress',
            'row-resolved': params =>params.data?.status === 'Resolved',
            'row-rejected': params =>params.data?.status === 'Rejected'
        },
        onFilterChanged: () => {
            const filterModel =gridApi.getFilterModel();
            if (!filterModel || Object.keys(filterModel).length === 0) {
                localStorage.removeItem("complaintFilter");
            }
            else {
                localStorage.setItem("complaintFilter",JSON.stringify(filterModel));
            }
        }
    };

    const gridDiv =document.querySelector('#myGrid');
    gridApi =agGrid.createGrid(gridDiv, gridOptions);
    loadComplaints();
});
function loadComplaints() {
    $.ajax({
        url: "/Complaint/GetComplaints",
        type: "GET",
        success: function (response) {

            userRole = response.role;
            gridApi.setGridOption("rowData", response.data);

            // Existing filters from localStorage
            let filterModel = {};

            let savedFilter = localStorage.getItem("complaintFilter");

            if (savedFilter) {
                filterModel = JSON.parse(savedFilter);
            }

            // Filters coming from chart click
            const params = new URLSearchParams(window.location.search);

            const status = params.get("status");
            const userName = params.get("userName");
            const category = params.get("category");

            if (status) {
                filterModel.status = {
                    filterType: "text",
                    type: "equals",
                    filter: status
                };
            }

            if (userName) {
                filterModel.userName = {
                    filterType: "text",
                    type: "equals",
                    filter: userName
                };
            }

            if (category) {
                filterModel.categoryName = {
                    filterType: "text",
                    type: "equals",
                    filter: category
                };
            }

            if (Object.keys(filterModel).length > 0) {

                gridApi.setFilterModel(filterModel);

                // Save merged filters
                localStorage.setItem(
                    "complaintFilter",
                    JSON.stringify(filterModel)
                );
            }
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
        `;

        if (row.status !== "Resolved") {

            html += `
                <button class="btn btn-success btn-sm"
                    onclick="updateStatus(${row.complaintId})">
                    <i class="bi bi-arrow-repeat"></i>
                    Update Status
                </button>
            `;
        }
    }

    // ADMIN
    else if (userRole === "Admin") {

        html += `
            <button class="btn btn-info btn-sm me-1"
                onclick="viewDetails(${row.complaintId})">
                <i class="bi bi-eye"></i> Details
            </button>
        `;

        if (row.status !== "Resolved") {

            html += `
                <button class="btn btn-success btn-sm me-1"
                    onclick="updateStatus(${row.complaintId})">
                    <i class="bi bi-arrow-repeat"></i>
                    Update Status
                </button>
            `;
        }

        if ((row.assignedTo == null || row.assignedTo === 0) && row.status !== "Resolved") {

            html += `
                <button class="btn btn-primary btn-sm"
                    onclick="assignComplaint(${row.complaintId})">
                    <i class="bi bi-person-plus"></i>
                    Assign Complaint
                </button>
            `;
        }
    }

    return html;
}

function viewDetails(id) {
    window.location.href =`/Complaint/Details?id=${id}`;
}
function editComplaint(id) {
    window.location.href = `/Complaint/Edit?id=${id}`;
}

function updateStatus(id) {
    window.location.href = `/Admin/Status/${id}`;
}
function assignComplaint(id) {
    window.location.href =`/Admin/Assign/${id}`;
}
function deleteComplaint(id) {
    if (!confirm("Are you sure you want to delete this complaint?"))
        return;
    $.ajax({
        url: "/Complaint/Delete",
        type: "POST",
        data: {
            id: id,
            __RequestVerificationToken:$('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            alert(response.message);
            if (response.success) {
                loadComplaints();
            }
        },
        error: function (xhr, status, error) {
            console.log("Status:", xhr.status);
            console.log("Response:", xhr.responseText);
            console.log("Error:", error);

            alert("Delete failed.");
        }
    });
}
function exportCSV() {
    gridApi.exportDataAsCsv({
        fileName: "ComplaintReport.csv"
    });
}
function formatDate(dateString) {
    if (!dateString)
        return "";
    let date = new Date(dateString);
    let day =String(date.getDate()).padStart(2, '0');
    let month =String(date.getMonth() + 1).padStart(2, '0');
    let year =date.getFullYear();
    let hours = String(date.getHours()).padStart(2, '0');
    let minutes =String(date.getMinutes()).padStart(2, '0');
    return `${day}-${month}-${year} ${hours}:${minutes}`;
}