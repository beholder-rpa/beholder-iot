import React, { useState, useEffect } from 'react';
import dynamic from 'next/dynamic';
import mqtt, { IMqttClient, ISubscriptionGrant} from 'async-mqtt';

function useInput({ type, initialState }) {
  const [value, setValue] = useState(initialState || '');
  const input = <input value={value} onChange={e => setValue(e.target.value)} type={type} />;
  return [value, input];
}

const Logs = dynamic(
  () => import('../components/LogsContainer'),
  { ssr: false }
)

export default function Home() {
  const [message, messageInput] = useInput({ type: "text", initialState: "Test Message" });
  let client: IMqttClient;

  useEffect(() => {
    let host = window.location.host;
    if (process.env.NODE_ENV !== 'production') {
      host = 'localhost:8080'
    }
    client = mqtt.connect(`ws://${host}/nexus/ws`, {
      username: "guest",
      password: "guest",
      reconnectPeriod: 5000,
      keepalive: 60,
    });
    
    let subscriptions: ISubscriptionGrant[];
    client.on("connect", (packet) => {
      console.log('%c connected!!', 'background: green; color: white; display: block;');
      client.subscribe("/topic/hello-from-dot-net", (_err, grant) => {
        subscriptions = grant;
      });
    });
    
    client.on("error", (err) => {
      console.log('Broker reported error: ' + err.message);
      console.log('Additional details: ' + err.stack);
    });
    
    return () => {
      client?.unsubscribe("/topic/hello-from-dot-net");
      client?.end();
    }
  });

  const sendMessage = (body) => {
    client?.publish("asdf", body);
  }

  return (
    <>
      <div>
        Send a message with MQTT: <br />
        {messageInput} <button onClick={() => sendMessage(message)}>Send</button> <br />
      </div>
      <div style={{marginTop: '15px', backgroundColor: '#242424', maxHeight: '500px', maxWidth: '600px', display: 'flex', flexDirection: 'column-reverse', overflow: 'scroll'}}>
        <Logs />
      </div>
    </>
  )
}
