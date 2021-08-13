const fetch = require('node-fetch');
const { baseUrl, stalkName } = require('../common/');

module.exports = function (RED) {
  function SendKeysRawNode(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', async function (msg) {
      await fetch(`${baseUrl}publish/nexus/beholder/stalk/${stalkName}/sendkeysraw`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(msg.payload),
      });
    });
  }
  RED.nodes.registerType("send-keys-raw", SendKeysRawNode);
}