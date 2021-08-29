import { observer } from 'mobx-react';

interface DaemonStatusProps {
  hostName: string;
}
const DaemonStatus = ({ hostName }: DaemonStatusProps) => {
  return (
    <div className="relative block w-full border-2 border-gray-300 border-dashed rounded-lg p-12 text-center hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
      {hostName}
      <img src={`/api/epidermis/mpeg/e/${hostName}/thumbnail`} alt="thumbnail" />
    </div>
  );
};

export default observer(DaemonStatus);
