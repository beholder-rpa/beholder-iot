import React from 'react';
import Image from 'next/image';
import { observer } from 'mobx-react';

import BeholderLogo from '@public/beholder.svg';
import Alerts from '@src/Layouts/CortexLayout/Alerts';
import Reminders from '@src/Layouts/CortexLayout/Reminders';
import UserWidget from '@src/Layouts/CortexLayout/UserProfile';
import { toBase64 } from '@utils/toBase64';
import { shimmer } from '@utils/shimmer';

import SearchBar from './SearchBar';

const AppHeader = () => {
  return (
    <div className="fixed w-full flex items-center justify-between h-14 z-10 bg-base-300">
      <div className="flex items-center justify-start pl-3 w-14 md:w-64 h-14 border-none">
        <Image
          width="40"
          height="40"
          className="w-7 h-7 md:w-10 md:h-10 Ks rounded-md"
          placeholder="blur"
          blurDataURL={`data:image/svg+xml;base64,${toBase64(shimmer(40, 40))}`}
          src={BeholderLogo}
          alt="Beholder Logo"
        />
        <span className="hidden md:block text-white ml-2">Beholder RPA</span>
      </div>
      <SearchBar />
      <Alerts />
      <Reminders />
      <UserWidget />
    </div>
  );
};

export default observer(AppHeader);
