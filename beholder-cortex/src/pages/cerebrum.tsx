import React, { useContext, useEffect, useState } from 'react';
import { observer } from 'mobx-react';
import dayjs from 'dayjs';

import CortexLayout from '@layouts/CortexLayout';
import { AppStoreContext } from '@stores/AppStore';

const Home = () => {
  return (
    <CortexLayout>
      <iframe className="h-full w-full" src="//cerebrum.beholder.localhost" title="Cerebrum" />
    </CortexLayout>
  );
};

export default observer(Home);
