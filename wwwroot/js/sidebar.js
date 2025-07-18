
    document.addEventListener("DOMContentLoaded", function () {
        const sidebarToggle = document.getElementById('sidebar-toggle');
    const sidebar = document.getElementById('sidebar');
    const wrapper = document.querySelector('.app__slide-wrapper');
        const subMenus = document.querySelectorAll('.sub-menu > a');

    sidebarToggle.addEventListener('click', function () {
        sidebar.classList.toggle('collapsed');
    wrapper.classList.toggle('sidebar-collapsed');
        });

        subMenus.forEach(menu => {
        menu.addEventListener('click', function (e) {
            e.preventDefault();
            const parent = this.parentElement;
            parent.classList.toggle('open');
        });
        });
    });