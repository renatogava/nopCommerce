$(document).ready(function () {

    // This is need so the content gets replaced correctly.
    $("#productdetails-window").on("show.bs.modal", function (e) {

        var productId = e.relatedTarget.attributes["data-productid"].value;

        $.ajax({
            cache: false,
            type: "GET",
            url: "CardapioOnline/GetProductDetails?productId=" + productId,
            success: function (response) {
                $(".modal-content").html(response);
            },
            error: function () {
                alert('error');
            }
        });
    });

    $("#productdetails-window").on("hide.bs.modal", function (e) {

        $(".modal-content").html("<div class=\"modal-header\"><button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button><h4>Carregando, por favor aguarde...</h4></div><div class=\"modal-body\"><div class=\"modal-busy\"><span class=\"loading\">&nbsp;</span></div></div>");
    });
});
