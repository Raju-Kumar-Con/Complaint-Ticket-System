$(document).ready(function () {
    loadComplaintChart("user");
    $("#chartFilter").on("change", function () {
        loadComplaintChart($(this).val());
    });
});
function loadComplaintChart(filterType) {
    $.ajax({
        url: "/Complaint/GetComplaintChartData",
        type: "GET",
        data: {
            filterType: filterType
        },
        success: function (data) {
            renderComplaintChart(data, filterType);
        },
        error: function (xhr, status, error) {
            console.error("Chart Load Error:", error);
        }
    });
}
function renderComplaintChart(data, filterType) {

    let grouped = {};

    data.forEach(function (item) {

        if (!grouped[item.name]) {

            grouped[item.name] = {
                name: item.name,
                Open: 0,
                "In Progress": 0,
                Resolved: 0,
                Rejected: 0
            };

        }
        grouped[item.name][item.status] = item.count;
    });
    const chartData = Object.values(grouped);
    const chartContainer = document.getElementById("myChart");
    if (!chartContainer) {
        console.error("myChart container not found");
        return;
    }
    chartContainer.innerHTML = "";
    agCharts.AgCharts.create(
        {
            container: chartContainer,
        title: {
            text: filterType === "user"
                ? "User Wise Complaints"
                : "Category Wise Complaints"
        },
        data: chartData,
        series: [
            {
                type: "bar",
                xKey: "name",
                yKey: "Open",
                yName: "Open"
            },
            {
                type: "bar",
                xKey: "name",
                yKey: "In Progress",
                yName: "In Progress"
            },
            {
                type: "bar",
                xKey: "name",
                yKey: "Resolved",
                yName: "Resolved"
            },
            {
                type: "bar",
                xKey: "name",
                yKey: "Rejected",
                yName: "Rejected"
            }
        ],

    listeners: {
        seriesNodeClick: function (event) {
        const status = event.yKey;
        const name = event.datum.name;
            if (filterType === "user") {
                window.location.href ="/Complaint/Index?status=" +encodeURIComponent(status) + "&userName=" +encodeURIComponent(name);
            }
            else {
                window.location.href = "/Complaint/Index?status=" + encodeURIComponent(status) + "&category=" +encodeURIComponent(name);
            }
        }
    }
    });
}