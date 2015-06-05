var PORT = 9000;
var HOST = '127.0.0.1';

var dgram = require('dgram');
var server = dgram.createSocket('udp4');

server.on('listening', function () {
    var address = server.address();
    console.log('OSK listening on ' + address.address + ":" + address.port);
});

server.on('message', function (message, remote) {
    console.log(message);
});

server.bind(PORT, HOST);