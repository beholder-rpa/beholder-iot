import React, { useEffect, useRef } from 'react';
import dynamic from 'next/dynamic';
import mqtt, { IMqttClient } from 'async-mqtt';
import { useForm } from 'react-hook-form';

import CortexLayout from '@src/Layouts/CortexLayout';
import { FormInput } from '@modules/Shared/FormInput';

const Logs = dynamic(() => import('../components/LogsContainer'), { ssr: false });

interface TestFormInputs {
  message: string;
}

export default function Home() {
  const {
    register,
    handleSubmit,
    formState: {},
  } = useForm<TestFormInputs>();
  const client = useRef<IMqttClient>();

  useEffect(() => {
    const host = window.location.host;
    client.current = mqtt.connect(`wss://${host}/nexus/ws`, {
      username: 'guest',
      password: 'guest',
      reconnectPeriod: 5000,
      keepalive: 60,
    });

    client.current.on('connect', (_packet) => {
      console.log('%c connected!!', 'background: green; color: white; display: block;');
      client.current.subscribe('/topic/hello-from-dot-net');
    });

    client.current.on('error', (err) => {
      console.log('Broker reported error: ' + err.message);
      console.log('Additional details: ' + err.stack);
    });

    return () => {
      client.current?.unsubscribe('/topic/hello-from-dot-net');
      client.current?.end();
    };
  }, []);

  const onSubmit = (data: TestFormInputs) => {
    client.current?.publish('asdf', data.message);
  };

  return (
    <CortexLayout>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <FormInput<TestFormInputs>
            field="message"
            label="Send a message with MQTT"
            placeholder="Enter a message to send with MQTT..."
            register={register}
          />
        </div>
        <button type="submit" className="btn btn-primary">
          Send
        </button>{' '}
        <br />
      </form>
      <div className="p-10 card bg-neutral-focus h-96">
        <Logs />
      </div>
    </CortexLayout>
  );
}
