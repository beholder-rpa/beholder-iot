import React, { useRef, useContext } from 'react';
import { observer } from 'mobx-react';

import CortexLayout from '@layouts/CortexLayout';
import { AppStoreContext } from '@stores/AppStore';
import { BeholderClientContext } from '@services/BeholderClient';
import mapKeyToKeypress from '@src/models/kinesis/mapKeyToKeypress';
import SimpleKeyboard from '@components/SimpleKeyboard';
import { Keyboard } from '@src/models/kinesis/Keyboard';

interface VirtualControlProps {
  hostName?: string;
}

const VirtualControls = ({ hostName }: VirtualControlProps) => {
  const { beholderStore } = useContext(AppStoreContext);
  const beholderClient = useContext(BeholderClientContext);
  const textAreaRef = useRef<HTMLTextAreaElement>();

  const kinesisHost = beholderStore.getFirstKinesisHost();

  if (!hostName) {
    const host = beholderStore.getFirstHost();
    if (host) {
      hostName = host.name;
    }
  }

  // TODO: Show LED status from state.
  // const _kinesisState = beholderStore.getKinesisState(kinesisHost?.name);

  const onChange = (input: string) => {
    const keyPress = mapKeyToKeypress(input);
    const keyboard = new Keyboard(beholderClient, kinesisHost?.name);
    keyboard.keypress(keyPress);
    textAreaRef.current.focus();
  };

  return (
    <CortexLayout>
      <div className="container">
        <div className="row justify-content-end">
          {/* <Dropdown>
            <Dropdown.Toggle variant="success" id="dropdown-basic">
              Kinesis Hosts
            </Dropdown.Toggle>

            <Dropdown.Menu>
              {beholderStore.kinesisHosts.map((host, ix) => (
                <Dropdown.Item key={ix}>{host.name}</Dropdown.Item>
              ))}
            </Dropdown.Menu>
          </Dropdown> */}
        </div>
        <div className="row">
          <textarea className="flex w-full" autoFocus ref={textAreaRef} rows={3} />
        </div>
        <div className="row">
          <SimpleKeyboard className="flex w-full" onKeyReleased={onChange} />
        </div>
      </div>
    </CortexLayout>
  );
};

export default observer(VirtualControls);
