document.addEventListener("DOMContentLoaded", function () {

    const columnDefs = [
        {
            headerName: "ID",
            field: "categoryId"
        },
        {
            headerName: "Category Name",
            field: "categoryName"
        },
        {
            headerName: "Status",
            field: "isActive",
            filter: false,
            cellRenderer: function (params) {

                if (params.value) {
                    return '<span class="badge bg-success">Active</span>';
                }

                return '<span class="badge bg-danger">Inactive</span>';
            }
        },
        {
            headerName: "Action",
            field: "categoryId",
            width: 250,
            sortable: false,
            filter: false,

            cellRenderer: function (params) {

                const buttonText = params.data.isActive
                    ? "Inactivate"
                    : "Activate";

                const buttonClass = params.data.isActive
                    ? "btn-danger"
                    : "btn-success";

                return `
        <a href="/Admin/EditCategory/${params.value}"
           class="btn btn-warning btn-sm me-2">
            <i class="bi bi-pencil-square"></i> Edit
        </a>

        <a href="/Admin/DeleteCategory/${params.value}"
           class="btn ${buttonClass} btn-sm"
           onclick="return confirm('Are you sure?')">
            ${buttonText}
        </a>
    `;
            }
        }
    ];

    const gridOptions = {

        columnDefs: columnDefs,

        rowData: [],

        defaultColDef: {
            sortable: true,
            filter: true,
            flex: 1,
            floatingFilter: true,
            resizable: true
        },
    };

    const gridDiv = document.querySelector("#myGrid");

    // AG Grid v36
    const gridApi = agGrid.createGrid(gridDiv, gridOptions);
    loadCategories();
    function loadCategories() {

        $.ajax({

            url: "/Admin/GetCategoryData",

            type: "GET",

            dataType: "json",

            success: function (response) {

                console.log("Data Received:", response);

                gridApi.setGridOption("rowData", response);
           
},

            error: function (xhr, status, error) {

                console.error(error);

                alert("Unable to load category data.");

            }

        });

    }

});