import React, { useContext } from 'react';
import dynamic from 'next/dynamic';
import { useForm } from 'react-hook-form';

import CortexLayout from '@layouts/CortexLayout';
import { FormInput } from '@modules/Shared/FormInput';
import { BeholderClientContext } from '@services/BeholderClient';

const Logs = dynamic(() => import('@components/LogsContainer'), { ssr: false });

interface TestFormInputs {
  message: string;
}

export default function Home() {
  const beholderClient = useContext(BeholderClientContext);
  const {
    register,
    handleSubmit,
    formState: {},
  } = useForm<TestFormInputs>();

  const onSubmit = (data: TestFormInputs) => {
    beholderClient.mqtt.publish('beholder', data.message);
  };

  return (
    <CortexLayout>
      <form className="pt-4 pl-5 mr-4 mb-4" onSubmit={handleSubmit(onSubmit)}>
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
      <div className="p-10 card bg-neutral-focus h-96 overflow-y-auto">
        <Logs />
      </div>
    </CortexLayout>
  );
}
