const { stalkName } = require('../common/');

module.exports = function (RED) {
  function SendKeysRawNode(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', async function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          bytes: config.buffer
        };
      }
    });

    this.send({
      topic: `beholder/stalk/${stalkName}/keyboard/raw`,
      payload: {
        datacontenttype: "application/json",
        specversion: "0.2",
        data: body
      }
    });
  }
  RED.nodes.registerType("send-keys-raw", SendKeysRawNode);
}