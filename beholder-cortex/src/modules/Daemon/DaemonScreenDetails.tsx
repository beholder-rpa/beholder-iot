import dayjs from 'dayjs';
import { observer } from 'mobx-react';

import { BeholderDaemonServiceInfo } from '@models/BeholderServiceInfo';

interface DaemonScreenDetailsProps {
  service: BeholderDaemonServiceInfo;
}

const DaemonScreenDetails = ({ service }: DaemonScreenDetailsProps) => {
  return (
    <div className="pt-4 pl-5 pr-4 pb-4 h-64 grid grid-rows-3 grid-flow-col gap-4">
      <div className="row-span-3 rounded-md flex justify-center items-center">
        <img src={`/api/epidermis/mpeg/e/${service.hostName}/thumbnail`} alt="thumbnail" />
      </div>
      <div className="col-span-2 bg-neutral-focus rounded-md flex justify-center items-center text-neutral-content text-2xl font-extrabold">
        {service.pointerPosition && (
          <>
            <span className="w-24">X: {service.pointerPosition.x}</span>
            <span className="w-24">Y: {service.pointerPosition.y}</span>
            <span className="w-24">V: {service.pointerPosition.v ? '+' : '-'}</span>
          </>
        )}
      </div>
      <div className="row-span-2 col-span-2 bg-base-200 rounded-md flex justify-center items-center text-base-contenttext-2xl font-extrabold">
        <div className="flex-1 flex items-center justify-center bg-base-500 rounded-md truncate">
          <div className="px-4 py-2 text-sm truncate">
            <span className="text-base-content">Pointer</span>
            <p>{service.pointerImage && <img src={service.pointerImage.image} alt="pointer" />}</p>
          </div>
        </div>
        <div className="flex-1 flex items-center justify-between bg-base-500 rounded-md truncate">
          <div className="px-4 py-2 text-sm truncate">
            <span className="text-base-content">Hotkey</span>
            <p>
              {service.lastHotKeyPressed && (
                <>
                  {service.lastHotKeyPressed.hotKey}
                  <br /> Time: {dayjs(service.lastHotKeyPressed.time).format('HH:mm:ss')}
                </>
              )}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default observer(DaemonScreenDetails);
