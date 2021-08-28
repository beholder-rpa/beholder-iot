module.exports = function (RED) {
  function StopObserving(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      
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
        topic: `beholder/eye/${hostName}/stop_observing`,
        payload: undefined
      });
    });
  }
  RED.nodes.registerType("stop-observing", StopObserving);
}