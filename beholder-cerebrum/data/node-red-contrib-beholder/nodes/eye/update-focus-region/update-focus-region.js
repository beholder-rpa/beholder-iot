module.exports = function (RED) {
  function UpdateFocusRegion(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          name: config.focusRegionName,
          kind: "image",
          bitmapSettings: {
            x: parseInt(config.focusRegionXPos),
            y:  parseInt(config.focusRegionYPos),
            width: parseInt(config.focusRegionWidth),
            height: parseInt(config.focusRegionHeight),
            maxFps: parseFloat(config.focusRegionMaxFps),
          }
        }
      }

      let hostName = msg.hostname || config.hostname;
      if (!hostName) {
        const beholderServices = globalContext.get('beholder_services');
        if (beholderServices && beholderServices.daemon) {
          hostName = beholderServices.daemon[0];
        }

        if (!hostName) {
          node.error('No daemon hostname specified and a daemon hostname could not be determined from the beholder_services global');
          return;
        }
      }
      
      this.send({
        topic: `beholder/eye/${hostName}/add_or_update_focus_region`,
        payload: body,
      });
    });
  }
  RED.nodes.registerType("update-focus-region", UpdateFocusRegion);
}