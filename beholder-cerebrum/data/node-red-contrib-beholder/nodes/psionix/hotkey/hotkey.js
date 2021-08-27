const { stalkName } = require('../common/');

module.exports = function (RED) {
  function SendMouseClickNode(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', async function (msg) {
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
      await fetch(`${baseUrl}publish/nexus/beholder/stalk/${stalkName}/mouse/click`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
      });
    });
  }
  RED.nodes.registerType("send-mouse-click", SendMouseClickNode);
}