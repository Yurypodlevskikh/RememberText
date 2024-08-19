(function ($) {
    'use strict';

    var anim;
    var addclass = function () {
        requestAnimationFrame(function () {
            $(".navbar-toggler").addClass("rt-show");
            $("#Logo_box_Intro").addClass("rt-show");
            $("#RT_Nav_Intro").addClass("rt-show");
            $("#Green_Line_Intro").addClass("rt-show");
            $("#Introduction").slideUp(2000);
            $("#Hide_Intro_Box").fadeOut(1000);
            $("#RT_navbar_Intro").addClass("rt-show");
            $("#ProjectsBox").addClass("rt-show");
            $("#RT_Footer").addClass("rt-show");
        });
    };

    clearTimeout(anim);
    anim = setTimeout(function () {
        addclass();
    }, 25000);

    $("#Hide_Intro").on("click", function (e) {
        e.preventDefault();
        $("#Hide_Intro_Box").slideUp(1000);
        addclass();
    });
    $("#ProjectsBox").on("submit", "form.likeform", function (e) {
        e.preventDefault();
        var that = $(this);
        var data = that.serialize();
        var url = that.attr("action");
        var like = that.find("span.project-likes").first();
        var btn = like.prev();
        var allLikes = like.text();
        $.post(url, data, function (response) {
            if (response.Success == true) {
                if (response.SuccessMessage !== undefined || response.SuccessMessage !== "") {
                    if (response.SuccessMessage > 0) {
                        if (btn.hasClass("btnlink-primary")) {
                            btn.switchClass("btnlink-primary", "btnlink-success", 500);
                        }
                        like.text(response.SuccessMessage);
                    } else {
                        if (allLikes !== "" && allLikes > 0) {
                            btn.switchClass("btnlink-success", "btnlink-primary", 500);
                        }
                        like.text("");
                    }
                }
            } else {
                if (response.ErrorMessage !== undefined || response.SuccessMessage !== "") {
                    window.location.href = response.ErrorMessage;
                }
            }
        });
    });

    $("#ProjectsBox").on("submit", "form.detailstextform", function (e) {
        e.preventDefault();
        if (e.handled !== true) {
            e.handled = true;
            var viewProjectForm = $(this);
            $.fn.formModal(viewProjectForm);
        }

        // How to display
        $("#RT_ModalForProject").on("click", "button.howtodisplaybtn", function (e) {
            e.preventDefault();
            var thatform = $(this).closest("form");
            var url = thatform.attr("action");
            var data = thatform.serialize();
            var dispbox = $("#DetailsTextMods");
            $("#textcontentpreload").show();
            $.post(url, data, function (response) {
                dispbox.html(response);
                $("#textcontentpreload").hide();
            });
        });
    });
}(jQuery));