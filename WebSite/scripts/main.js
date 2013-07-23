/* Author:Melnichuk Vladimir, Artur Liahiv*/
var ie8 = ($.browser.msie && $.browser.version == '8.0') ? true : false;

function init_responsive_menu() {
	var menu = $('.menu nav').html();
	var r_menu = '<div class="r_menu">';
	r_menu += '<div class="r_menu_content">';
	r_menu += '<div class="arrow"></div>';
	r_menu += '<div class="text">目录</div>';
	r_menu += '</div>';
	r_menu += menu;
	r_menu += '</div>';
	$('.menu').after(r_menu);

	$('.r_menu .r_menu_content').live('click', function() {
		$(this).toggleClass('collapsed');
		$('.r_menu > ul').slideToggle(300);
	});

	$('.menu nav li a, .r_menu li a').live('click', function(e) {
		if($(this).attr('href') == '#' || $(this).attr('href') == '') {
			e.preventDefault();
		}
	});
}

function init_fields() {
	$('.w_def_text').each(function() {
		var text = $(this).attr('title');
		
		if($(this).val() == '') {
			$(this).val(text);
		}
	});
	
	$('.w_def_text').live('click', function() {
		var text = $(this).attr('title');
		
		if($(this).val() == text) {
			$(this).val('');
		}
		
		$(this).focus();
	});
	
	$('.w_def_text').live('blur', function() {
		var text = $(this).attr('title');
		
		if($(this).val() == '') {
			$(this).val(text);
		}
	});
}

function init_pricing_table() {
	$('.block_pricing_table_1, .block_pricing_table_2').each(function() {
		var content = $(this).html();
		var native_class = $(this).attr('class');
		var new_content = '<div class="' + native_class + ' responsive flexslider"><ul class="slides">';
		new_content += content;
		new_content += '</ul></div>';

		$(this).after(new_content);
	});

	$('.block_pricing_table_1.responsive, .block_pricing_table_2.responsive').each(function() {
		$(this).find('.clear').remove();
		$(this).find('.column').wrap('<li />');
	});

	$('.block_pricing_table_1.responsive, .block_pricing_table_2.responsive').flexslider({
		animation : 'slide',
		slideshow : false,
		controlNav : false,
		smoothHeight : true
	});
}

function init_footer_tooltip() {
	$('.footer_tooltip').each(function(){
		var text = $(this).html();
		var content = '<span class="f_tooltip"><span class="arrow"></span>' + text + '</span>';
		$(this).html(content);
	});

	$('.footer_tooltip .f_tooltip').each(function(){
		var left = $(this).outerWidth() / 2;
		$(this).css('margin-left', '-' + left + 'px');
	});
}





jQuery(document).ready(function($) {
	$('.promo_slider').slides({
		effect: 'fade',
		fadeSpeed: 700,
		play: 4000,
		pause: 1000,
		hoverPause: true,
		generateNextPrev: false,
		generatePagination: false,
		autoHeight: true		
	});

	init_fields();
	init_footer_tooltip();

	if(!ie8) {
		init_responsive_menu();
		init_pricing_table();
	}
});






