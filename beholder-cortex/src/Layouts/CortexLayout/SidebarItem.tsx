import React, { ReactNode } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';

import classNames from '@src/utils/classNames';

type Props = {
  title?: string;
  icon?: ReactNode;
  url: string;
  className?: string;
};

const SidebarItem = ({ title = 'Title', icon, url, className }: Props) => {
  const router = useRouter();
  let highlight = '';
  if (router.asPath === url) {
    highlight = 'border-skin-300 bg-base-300';
  }
  return (
    <Link href={url} passHref={true}>
      <a
        className={classNames(
          'relative flex flex-row items-center h-11 hover:bg-skin-600 dark:hover:bg-skin-700 text-gray-200 hover:text-gray-100 border-l-4 border-transparent hover:border-skin-200 dark:hover:border-skin-400 pr-6',
          className,
          highlight,
        )}
      >
        <span className="inline-flex justify-center items-center ml-3">{icon}</span>
        <span className="ml-2 text-sm tracking-wide truncate">{title}</span>
      </a>
    </Link>
  );
};

export default SidebarItem;
