module.exports = function (RED) {
  function RegisterHotKey(config) {
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
        topic: `beholder/psionix/${config.hostName}/hotkeys/register`,
        payload: body
      });
    });
  }
  RED.nodes.registerType("register-hotkey", RegisterHotKey);
}