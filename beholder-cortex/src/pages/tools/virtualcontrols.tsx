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
  const {} = useContext(AppStoreContext);
  const beholderClient = useContext(BeholderClientContext);
  const textAreaRef = useRef<HTMLTextAreaElement>();

  const onChange = (input: string) => {
    const keyPress = mapKeyToKeypress(input);
    const keyboard = new Keyboard(beholderClient, hostName);
    keyboard.keypress(keyPress);
    textAreaRef.current.focus();
  };

  return (
    <CortexLayout>
      <div className="pt-4 pl-5 mr-4 mb-4 container">
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
          <textarea className="textarea h-24 flex w-full textarea-bordered" autoFocus ref={textAreaRef} rows={3} />
        </div>
        <div className="row">
          <SimpleKeyboard className="flex w-full" onKeyReleased={onChange} />
        </div>
      </div>
    </CortexLayout>
  );
};

export default observer(VirtualControls);
