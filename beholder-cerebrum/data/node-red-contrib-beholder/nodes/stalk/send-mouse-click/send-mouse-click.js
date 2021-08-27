const { stalkName } = require('../common/');

module.exports = function (RED) {
  function SendMouseClick(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          mouse_click: {
            button: config.button,
            click_direction: config.click_direction,
            duration: config.duration,
          }
        };
      }
      this.send({
        topic: `beholder/stalk/${stalkName}/mouse/click`,
        payload: {
          datacontenttype: "application/json",
          specversion: "0.2",
          data: body
        }
      });
    });
  }
  RED.nodes.registerType("send-mouse-click", SendMouseClick);
}