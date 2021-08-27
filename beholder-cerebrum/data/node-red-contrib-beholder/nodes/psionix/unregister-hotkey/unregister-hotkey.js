module.exports = function (RED) {
  function UnregisterHotKey(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          keys: `${config.modifiers}${config.key}`
        };
      }
      this.send({
        topic: `beholder/psionix/${config.hostname}/hotkeys/unregister`,
        payload: body
      });
    });
  }
  RED.nodes.registerType("unregister-hotkey", UnregisterHotKey);
}