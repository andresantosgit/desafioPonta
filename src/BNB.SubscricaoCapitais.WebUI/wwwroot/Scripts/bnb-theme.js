///ativar o componente dropdown
$('.ui.dropdown').dropdown({ fullTextSearch: true });
//ativar o componente popup
$('.button').popup();
//ativar o componente accordion
$('.ui.accordion').accordion();
//ativar o componente toggle
$('.ui.checkbox').checkbox();
//ativar o componente tabs
$('.tabular.menu .item, .ui.pointing.secondary.menu .item').tab();

//transformar o meu vertical em slider
$('.ui.sidebar').sidebar({
    context: $('.bottom.segment')
  }).sidebar('setting', 'transition', 'overlay')
    .sidebar('attach events', '.sidebar.item');

$('.hasDatepicker').addClass('ui-inputtext');

$(document).ready(function() {	

	//estender o menu vertical na altura do document
	$(window).resize(function() {
    	var valueHeight = $(window).height();
		$('.pusher').css('min-height', (valueHeight - 74) + 'px');
	}).resize();
	

	//alterar ícone ao abrir o menu
	$('.item.sidebar .arrow').hide();
	$('.item.sidebar').on('click', function(){
		
		if( $(this).attr('data-click-state') == 1) {			
			$(this).attr('data-click-state', 0);
			$('.item.sidebar .sidebar').show();
			$('.item.sidebar .arrow').hide();
		} else {
			$(this).attr('data-click-state', 1);		
			$('.item.sidebar .sidebar').hide();			
			$('.item.sidebar .arrow').show();			
		}

	});
	$('.pusher').on('click', function(){
		if( $('.item.sidebar').attr('data-click-state') == 1 ){
			$('.item.sidebar').attr('data-click-state', 0);
			$('.item.sidebar .sidebar').show();
			$('.item.sidebar .arrow').hide();
		}
	});
	

});

