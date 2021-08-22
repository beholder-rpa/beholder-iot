import React from 'react';
import { observer } from 'mobx-react';

import CortexLayout from '@layouts/CortexLayout';

const Home = () => {
  return (
    <CortexLayout>
      <iframe className="h-full w-full" src="//cerebrum.beholder.localhost" title="Cerebrum" />
    </CortexLayout>
  );
};

export default observer(Home);
