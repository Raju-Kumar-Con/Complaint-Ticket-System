document.addEventListener("DOMContentLoaded", () => {

    const sidebar = document.getElementById("sidebar");
    const mainContent = document.getElementById("mainContent");
    const toggleBtn = document.getElementById("toggleBtn");

    if (!sidebar || !mainContent || !toggleBtn) return;

    toggleBtn.addEventListener("click", () => {

        if (window.innerWidth <= 768) {

            sidebar.classList.toggle("show");

        } else {

            sidebar.classList.toggle("collapsed");
            mainContent.classList.toggle("expand");

        }

    });

    // Close mobile sidebar when clicking outside
    document.addEventListener("click", (e) => {

        if (
            window.innerWidth <= 768 &&
            sidebar.classList.contains("show") &&
            !sidebar.contains(e.target) &&
            !toggleBtn.contains(e.target)
        ) {
            sidebar.classList.remove("show");
        }

    });

    // Reset sidebar state when resizing
    window.addEventListener("resize", () => {

        if (window.innerWidth > 768) {
            sidebar.classList.remove("show");
        }

    });

});