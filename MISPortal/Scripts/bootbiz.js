/*
 * BootBiz - Multi Purpose Bootstrap Theme v1
 * Bootstrap v2.2.2
 * Author : Raj (www.madebyraj.com)
 * Copyright 2013 DoozyPixels
 * Designed and Created For Designers and Developers.
 * Easily Built Corporate, Product, Blog, Client Admin and more... templates
 * more option in future updates
 */

//on click show Tab content
bb_tabs = function(tabs_selector){
$(tabs_selector).on('click',function(e) {
	  e.preventDefault();
	  $(this).tab('show');
});		
}

//add arrow up/down for accordion panel
bb_accordion = function(accordion_selector){
$(accordion_selector).on({
	shown :function(e){
	$(e.target).siblings(".accordion-heading").find(".accordion-toggle i").removeClass("icon-chevron-right").addClass("icon-chevron-down");
	},
	hidden :function(e){
	$(e.target).siblings(".accordion-heading").find(".accordion-toggle i").removeClass("icon-chevron-down").addClass("icon-chevron-right");
	}
});
}

//on scroll navbar fixedTop 
bb_navbar_affix = function(affix_selector){
	$(affix_selector).affix(function(){
		offset: 0
	});
}
bb_maps_initialize = function() {
	//Google maps
var place = new google.maps.LatLng(59.32522, 18.07002),//Change your Lat and Long
	custom_marker = 'assets/img/marker-icon.png',//Change your marker_icon
	contentString = '<div id="content">'+
            '<address><b>BootBiz, Inc</b><br/>'+
            '123 Street Name, 101 Suite<br/>'+
			'Cityname<br/>'+                    
            'State - 12345.</address>'+
            '</div>',
   	marker,
	map;
	
	var mapOptions = {
	  zoom: 13,
	  mapTypeId: google.maps.MapTypeId.ROADMAP,
	  center: place
	};
	map = new google.maps.Map(document.getElementById('map_canvas'),mapOptions);
	var infowindow = new google.maps.InfoWindow({
		content: contentString
	});		
	marker = new google.maps.Marker({
	  map:map,
	  draggable:false,
	  position: place,
	  title: 'Our Office',
	  icon: custom_marker
	  
	});
	google.maps.event.addListener(marker, 'click', function() {
	  infowindow.open(map,marker);
	});
}
