module.exports = function (RED) {
  function StartObserving(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          adapterIndex: parseInt(config.adapterIndex),
          deviceIndex: parseInt(config.deviceIndex),
          regions: [{
            name: "default",
            kind: "image",
            bitmapSettings: {
              x: 0,
              y: 0,
              width: 640,
              height: 480,
              maxFps: 0.25,
            }
          }],
          streamDesktopThumbnail: !!config.streamDesktopThumbnail,
          streamPointerImage: !!config.streamPointerImage,
          desktopThumbnailStreamSettings: {
            maxFps: parseFloat(config.desktopThumbnailStreamSettingsMaxFps),
            scaleFactor: parseFloat(config.desktopThumbnailStreamSettingsScaleFactor),
          },
          pointerImageStreamSettings: {
            maxFps: parseFloat(config.pointerImageStreamSettingsMaxFps),
          },
          watchPointerPosition: !!config.watchPointerPosition,
        }
      }

      let hostName = config.hostname;
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
        topic: `beholder/eye/${hostName}/start_observing`,
        payload: body
      });
    });
  }
  RED.nodes.registerType("eye-start-observing", StartObserving);
}