/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    usepopupnotifications: false,
    topcartselector: '',
    topwishlistselector: '',
    flyoutcartselector: '',
    localized_data: false,

    init: function (usepopupnotifications, topcartselector, topwishlistselector, flyoutcartselector, localized_data) {
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
        this.localized_data = localized_data;
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
    },

    setLoadWaitingModal: function (display) {
      displayAjaxLoadingModal(display);
      this.loadWaiting = display;
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            type: "POST",
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            data: $(formselector).serialize(),
            type: "POST",
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    //add a product to the cart modal page
    addproducttocart_details_modal: function (urladd, formselector) {
      if (this.loadWaiting !== false) {
        return;
      }
      this.setLoadWaitingModal(true);

      $.ajax({
        cache: false,
        url: urladd,
        data: $(formselector).serialize(),
        type: "POST",
        success: this.success_process_modal,
        complete: this.resetLoadWaiting_modal,
        error: this.ajaxFailure
      });
    },

    //add a product to compare list
    addproducttocomparelist: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            type: "POST",
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    success_process: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success === true) {
                //success
                if (AjaxCart.usepopupnotifications === true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    displayBarNotification(response.message, 'success', 3500);
                }
            }
            else {
                //error
              if (AjaxCart.usepopupnotifications === true) {
                displayPopupNotification(response.message, 'error', true);
              }
              else {
                //no timeout for errors
                displayBarNotification(response.message, 'error', 0);
              }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
        AjaxCart.setLoadWaiting(false);
    },

    resetLoadWaiting_modal: function () {
      AjaxCart.setLoadWaitingModal(false);
    },

    success_process_modal: function (response) {
      if (response.updatetopcartsectionhtml) {
        $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
      }
      if (response.updatetopwishlistsectionhtml) {
        $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
      }
      if (response.updateflyoutcartsectionhtml) {
        $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
      }
      if (response.message) {
        //display notification
        if (response.success === true) {
          //success
          $(".modal-content").html("<div class=\"modal-header\"><button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button><h4>Seu produto foi adicionado na sacola</h4></div><div class=\"modal-body\"><div><a class=\"add-more-itens-modal\" data-dismiss=\"modal\">Adicionar mais itens</a><input class=\"submit-order-modal\" type=\"button\" onclick=\"location.href = '/cart'\" value=\"Finalizar Pedido\" /></div></div>");
        }
        else {
          //error
          //no timeout for errors
          displayBarNotificationModal(response.message, 'error', 0);
        }
        return false;
      }
      if (response.redirect) {
        location.href = response.redirect;
        return true;
      }
      return false;
    },

    ajaxFailure: function () {
        alert(this.localized_data.AjaxCartFailure);
    }
};