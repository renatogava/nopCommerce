function categoryScroll(id) {

  $('html, body').animate({
    scrollTop: $("#cat-" + id).offset().top - 55
  }, 1000);

}
