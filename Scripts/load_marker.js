
var geocoder;
var map;
var markers = [];
var minZoomLevel = 12, maxZoomLevel = 17;
var infowindow = new google.maps.InfoWindow();

function initialize() {
    // load map
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(10.8230989, 106.6296638);
    var mapOptions = {
        zoom: minZoomLevel,
        center: latlng,
        mapTypeId: 'roadmap'
    }
    map = new google.maps.Map(document.getElementById('map'), mapOptions);

    codeAddress();

    // Bounds for North America
    var strictBounds = new google.maps.LatLngBounds(

     new google.maps.LatLng(10.0230989, 106.0296638),
     new google.maps.LatLng(10.9930989, 106.8296638));

    // Listen for the dragend event
    google.maps.event.addListener(map, 'dragend', function () {
        var c = map.getCenter(),
                x = c.lng(),
                y = c.lat(),
                bounds = map.getBounds(),
                ne = bounds.getNorthEast(),
                sw = bounds.getSouthWest(),
                maxX = ne.lng(),
                maxY = ne.lat(),
                minX = sw.lng(),
                minY = sw.lat();

        if ((x > minX || x < maxX) || (y < minY || y > maxY)) {
            map.setCenter(new google.maps.LatLng(y, x));
        }
        else {

        }

    });

    // Limit the zoom level
    google.maps.event.addListener(map, 'zoom_changed', function () {
        if (map.getZoom() < minZoomLevel) map.setZoom(minZoomLevel);
        if (map.getZoom() > maxZoomLevel) map.setZoom(maxZoomLevel);
    });
}

/***** delete markers *****/
function setAllMap(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}
function deleteMarkers() {
    setAllMap(null);
    markers = [];
}
/***** end delete markers *****/

function codeAddress() {
    // delete marker
    deleteMarkers();
    //
    var address = document.getElementById('AddressMap').value;
    if (address != "") {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location,
                    animation: google.maps.Animation.DROP,
                    draggable: true,
                });
                /***** get marker's address *****/
                geocoder.geocode({ 'latLng': marker.getPosition() }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        if (results[0]) {
                            //document.getElementById('txtAddress').value = results[0].formatted_address;
                            infowindow.setContent(address);
                            infowindow.open(map, marker);
                        }
                    }
                });
                /***** end get marker's address *****/
                // get position marker
                document.getElementById('xMap').value = marker.getPosition().lat();
                document.getElementById('yMap').value = marker.getPosition().lng();
                //
                // add marker to array markers
                markers.push(marker);
                //
                (function (marker) {
                    google.maps.event.addListener(marker, 'drag', function () {
                        updateMarkerPosition(marker.getPosition());
                    });

                    google.maps.event.addListener(marker, 'dragend', function () {
                        geocodePosition(marker.getPosition());
                        //getAddressMarker(marker);

                        /***** get marker's address *****/
                        geocoder.geocode({ 'latLng': marker.getPosition() }, function (results, status) {
                            if (status == google.maps.GeocoderStatus.OK) {
                                if (results[0]) {
                                    //document.getElementById('txtAddress').value = results[0].formatted_address;
                                    //								infowindow.setContent(results[0].formatted_address);
                                    //								infowindow.open(map, marker);
                                    infowindow.setContent(address);
                                    //infowindow.open(map, marker);
                                }
                            }
                        });
                        /***** end get marker's address *****/
                    });
                })(marker);
            } else {
                alert('Không load được địa chỉ: ' + status);
            }
        });
    }
}

/***** get marker's position  *****/
function geocodePosition(pos) {
    geocoder.geocode({
        latLng: pos
    }, function (responses) {
        if (responses && responses.length > 0) {
            updateMarkerAddress(responses[0].formatted_address);
        } else {
            updateMarkerAddress('Không thể định vị được địa chỉ.');
        }
    });
}
function updateMarkerPosition(latLng) {
    document.getElementById('xMap').value = latLng.lat();
    document.getElementById('yMap').value = latLng.lng();
}
function updateMarkerAddress(str) {
    //document.getElementById('txtAddress').value = str;
}
/***** end get marker's position  *****/

function setCenter(latlng) {
    var z = Math.pow(2, map.getZoom());
    var pnt = map.getProjection().fromLatLngToPoint(latlng);
    map.setCenter(map.getProjection().fromPointToLatLng(
                                    new google.maps.Point(pnt.x + 150 / z, pnt.y)));
}

function setBounds(bnd, cb) {
    var prj = map.getProjection();
    if (!bnd) bnd = map.getBounds();
    var ne = prj.fromLatLngToPoint(bnd.getNorthEast()),
            sw = prj.fromLatLngToPoint(bnd.getSouthWest());
    if (cb) ne.x += (300 / Math.pow(2, map.getZoom()));
    else google.maps.event.addListenerOnce(map, 'bounds_changed',
            function () { setBounds(bnd, 1) });
    map.fitBounds(new google.maps.LatLngBounds(
            prj.fromPointToLatLng(sw), prj.fromPointToLatLng(ne)));
}

google.maps.event.addDomListener(window, 'load', initialize);