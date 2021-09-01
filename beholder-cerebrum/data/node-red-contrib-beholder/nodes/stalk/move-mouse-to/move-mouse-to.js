const { stalkName } = require('../common/');

module.exports = function (RED) {
  function MoveMouseTo(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    const globalContext = this.context().global;
    node.on('input', function (msg) {
      let body = {};
      if (msg.hasOwnProperty("payload") && msg.payload) {
        body = msg.payload;
      } else {
        let currentPosition = { x: 0, y: 0 };
        const globalCurrentPosition = globalContext.get('beholder_pointer_current_position');
        if (globalCurrentPosition) {
          currentPosition.x = globalCurrentPosition.x;
          currentPosition.y = globalCurrentPosition.y;
        }

        let targetPosition = { x: 0, y: 0 };
        const globalTargetPosition = globalContext.get('beholder_pointer_target_position');
        if (globalTargetPosition) {
          targetPosition.x = globalTargetPosition.x;
          targetPosition.y = globalTargetPosition.y;
        } else {
          targetPosition.x = parseInt(config.targetPositionX);
          targetPosition.y = parseInt(config.targetPositionY);
        }

        body = {
          current_position: currentPosition,
          target_position: targetPosition,
          movement_type: 0,
          movement_speed: parseInt(config.movementSpeed) || 2,
          movement_scale_x: parseInt(config.movementScaleX) || 1,
          movement_scale_y: parseInt(config.movementScaleY) || 1,
          pre_move_actions: config.preMoveActions || "",
          post_move_actions: config.postMoveActions || "",
        }
      }
      this.send({
        topic: `beholder/stalk/${stalkName}/mouse/move_mouse_to`,
        payload: {
          datacontenttype: "application/json",
          specversion: "0.2",
          data: body
        }
      });
    });
  }
  RED.nodes.registerType("move-mouse-to", MoveMouseTo);
}