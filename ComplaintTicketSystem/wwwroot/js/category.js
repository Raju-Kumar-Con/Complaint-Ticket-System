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

                    <button type="button"
                            class="btn ${buttonClass} btn-sm toggle-category"
                            data-id="${params.value}"
                            style="width:110px;">
                        ${buttonText}
                    </button>
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
        }
    };

    const gridDiv = document.querySelector("#myGrid");

    const gridApi = agGrid.createGrid(gridDiv, gridOptions);

    loadCategories();

    function loadCategories() {

        $.ajax({

            url: "/Admin/GetCategoryData",

            type: "GET",

            dataType: "json",

            success: function (response) {

                console.log(response);

                gridApi.setGridOption("rowData", response);

            },

            error: function () {

                alert("Unable to load category data.");

            }

        });

    }

    // Toggle Active/Inactive
    $(document).on("click", ".toggle-category", function () {

        if (!confirm("Are you sure?")) {
            return;
        }

        const id = $(this).data("id");

        const token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({

            url: "/Admin/ToggleCategoryStatus",

            type: "POST",

            data: {
                id: id,
                __RequestVerificationToken: token
            },

            success: function () {

                loadCategories();

            },

            error: function () {

                alert("Unable to update category status.");

            }

        });

    });

});