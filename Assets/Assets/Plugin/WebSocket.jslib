mergeInto(LibraryManager.library, {
  WebSocketConnect: function(url) {
    var urlStr = UTF8ToString(url);
    window.socket = new WebSocket(urlStr);
    
    window.socket.onopen = function() {
      console.log('Connected to WebSocket server');
    };

    window.socket.onmessage = function(event) {
      console.log('Received message:', event.data);
      // You may want to call a C# method here to process the message
    };

    window.socket.onerror = function(error) {
      console.error('WebSocket error:', error);
    };

    window.socket.onclose = function(event) {
      console.log('WebSocket connection closed:', event.code, event.reason);
    };
  },

  WebSocketSend: function(message) {
    var messageStr = UTF8ToString(message);
    if (window.socket && window.socket.readyState === WebSocket.OPEN) {
      window.socket.send(messageStr);
    } else {
      console.error('WebSocket is not open');
    }
  },

  WebSocketClose: function() {
    if (window.socket) {
      window.socket.close();
    }
  }
});
