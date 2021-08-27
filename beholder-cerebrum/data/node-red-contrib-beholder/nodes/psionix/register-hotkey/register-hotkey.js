module.exports = function (RED) {
  function RegisterHotKey(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const context = this.context();
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = `${config.modifiers}${config.key}`;
      }
      let hostName = config.hostname;
      if (!hostName) {
        const beholderServices = globalContext.get('beholder_services');
        if (beholderServices && beholderServices.psionix) {
          hostName = beholderServices.psionix[0];
        }

        if (!hostName) {
          node.error('No psionix hostname specified and a psionix hostname could not be determined from the beholder_services global');
          return;
        }
      }
      this.send({
        topic: `beholder/psionix/${hostName}/hotkeys/register`,
        payload: body
      });
    });
  }
  RED.nodes.registerType("register-hotkey", RegisterHotKey);
}