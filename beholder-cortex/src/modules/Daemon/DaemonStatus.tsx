import { observer } from 'mobx-react';
import { useState } from 'react';

import { BeholderDaemonServiceInfo } from '@models/BeholderServiceInfo';
import classNames from '@utils/classNames';

import DaemonScreenDetails from './DaemonScreenDetails';

interface DaemonStatusProps {
  service: BeholderDaemonServiceInfo;
}

const DaemonStatus = ({ service }: DaemonStatusProps) => {
  const [activeTab, setActiveTab] = useState<'screen' | 'regions' | 'processes'>('screen');
  return (
    <div className="relative block w-full border-2 border-gray-300 border-dashed rounded-lg p-4 hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
      <span>{service.hostName}</span>
      <div className="tabs">
        <a
          className={classNames('tab tab-bordered', activeTab === 'screen' ? 'tab-active' : '')}
          onClick={() => setActiveTab('screen')}
        >
          Screen
        </a>
        <a
          className={classNames('tab tab-bordered', activeTab === 'regions' ? 'tab-active' : '')}
          onClick={() => setActiveTab('regions')}
        >
          Regions
        </a>
        <a
          className={classNames('tab tab-bordered', activeTab === 'processes' ? 'tab-active' : '')}
          onClick={() => setActiveTab('processes')}
        >
          Processes
        </a>
      </div>
      {activeTab === 'screen' && <DaemonScreenDetails service={service} />}
      {activeTab === 'regions' && (
        <ol>
          {Object.keys(service.regions).map((regionName) => (
            <li key={regionName}>
              <a
                href={`/api/epidermis/mpeg/${service.regions[regionName].prefrontalImageKey}`}
                target="_blank"
                rel="noreferrer"
              >
                {regionName}
              </a>
            </li>
          ))}
        </ol>
      )}
    </div>
  );
};

export default observer(DaemonStatus);
