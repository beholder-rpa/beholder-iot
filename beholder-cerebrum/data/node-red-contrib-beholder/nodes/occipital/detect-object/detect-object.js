const { occipitalName } = require('../common/');

module.exports = function (RED) {
  function DetectObject(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          queryImagePrefrontalKey: config.queryImagePrefrontalKey,
          targetImagePrefrontalKey: config.targetImagePrefrontalKey,
          matchMaskSettings: {
            RatioThreshold: parseFloat(config.matchRatioThreshold),
            ScaleIncrement: parseFloat(config.scaleIncrement),
            RotationBins: parseInt(config.rotationBins),
          },
          outputImagePrefrontalKey: config.outputImagePrefrontalKey,
        };
      }
      this.send({
        topic: `beholder/occipital/DESKTOP-K4T6J4J/object_detection/detect`,
        payload: {
          datacontenttype: "application/json",
          specversion: "0.1",
          data: body
        }
      });
    });
  }
  RED.nodes.registerType("detect-object", DetectObject);
}