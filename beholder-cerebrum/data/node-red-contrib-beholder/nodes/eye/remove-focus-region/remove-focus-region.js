module.exports = function (RED) {
  function RemoveFocusRegion(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = config.focusRegionName;
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
        topic: `beholder/eye/${hostName}/remove_focus_region`,
        payload: body,
      });
    });
  }
  RED.nodes.registerType("remove-focus-region", RemoveFocusRegion);
}