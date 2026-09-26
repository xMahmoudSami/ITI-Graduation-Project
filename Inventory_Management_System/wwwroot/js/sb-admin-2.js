(function($) {
  "use strict"; // Start of use strict

  // Toggle the side navigation
  $("#sidebarToggle, #sidebarToggleTop").on('click', function(e) {
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");
    if ($(".sidebar").hasClass("toggled")) {
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Close any open menu accordions when window is resized below 768px
  $(window).resize(function() {
    if ($(window).width() < 768) {
      $('.sidebar .collapse').collapse('hide');
    };
    
    // Toggle the side navigation when window is resized below 480px
    if ($(window).width() < 480 && !$(".sidebar").hasClass("toggled")) {
      $("body").addClass("sidebar-toggled");
      $(".sidebar").addClass("toggled");
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
  $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function(e) {
    if ($(window).width() > 768) {
      var e0 = e.originalEvent,
        delta = e0.wheelDelta || -e0.detail;
      this.scrollTop += (delta < 0 ? 1 : -1) * 30;
      e.preventDefault();
    }
  });

  // Remove scroll-to-top button completely
  function purgeScrollToTop() {
    $('.scroll-to-top, a[href*="page-top"]').remove();
  }
  purgeScrollToTop();
  $(document).ready(purgeScrollToTop);
  $(window).on('scroll', purgeScrollToTop);

  // Active sidebar page highlight
  function markActive() {
    var rawPath = window.location.pathname || "";
    var cur = rawPath.replace(/\/+$/, "").toLowerCase();
    var isDashboardPage = (cur === "" || cur === "/" || cur === "/dashboard" || cur === "/dashboard/analytics");

    var links = document.querySelectorAll(".sidebar .nav-item a.nav-link");
    links.forEach(function(a) {
      if (a.hasAttribute("data-toggle")) return;
      var rawHref = a.getAttribute("href") || "";
      var href = rawHref.split("?")[0].replace(/\/+$/, "").toLowerCase();
      var isDashboardLink = (href === "" || href === "/" || href === "/dashboard" || href === "/dashboard/analytics");

      var isMatch = false;
      if (isDashboardPage) {
        if (isDashboardLink) isMatch = true;
      } else if (!isDashboardLink && href && href !== "#") {
        if (cur === href || cur.indexOf(href + "/") === 0 || cur.indexOf(href) === 0) {
          isMatch = true;
        }
      }

      if (isMatch) {
        a.classList.add("active-page");
        var li = a.closest(".nav-item");
        if (li && !a.closest(".collapse")) {
          li.classList.add("active-page");
        }
        var col = a.closest(".collapse");
        if (col && typeof jQuery !== "undefined") {
          jQuery(col).addClass("show");
          if (li) {
            var toggle = li.querySelector(':scope > a[data-toggle="collapse"]');
            if (toggle) {
              toggle.classList.remove("collapsed");
              toggle.setAttribute("aria-expanded", "true");
            }
          }
        }
      }
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", markActive);
  } else {
    markActive();
  }

})(jQuery); // End of use strict
