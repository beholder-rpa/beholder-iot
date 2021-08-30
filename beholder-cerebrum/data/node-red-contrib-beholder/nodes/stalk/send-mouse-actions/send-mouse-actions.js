const { stalkName } = require('../common/');

module.exports = function (RED) {
  function SendMouseActions(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          actions: config.actions,
        }
      }
      this.send({
        topic: `beholder/stalk/${stalkName}/mouse/actions`,
        payload: {
          datacontenttype: "application/json",
          specversion: "0.2",
          data: body
        }
      });
    });
  }
  RED.nodes.registerType("send-mouse-actions", SendMouseActions);
}